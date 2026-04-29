using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using AINotesHub.Shared.DTOs;
using AINotesHub.WPF.Helpers;
using AINotesHub.WPF.Properties;
using AINotesHub.WPF.Services;
using AINotesHub.WPF.UserControls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using Serilog;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace AINotesHub.WPF
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly HttpClient _httpClient;
        public LoginWindow()
        {
            InitializeComponent();
            // ⚠️ Replace this URL with your API base address
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44357/")
            };
            // Get the current window's AppWindow
            //var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            //AppWindow appWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(hwnd));

            //ApplicationView.GetForCurrentView().Title = "Custom text";
            //var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            //titleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 242, 221, 231);

            //// Set active window colors
            //titleBar.ForegroundColor = Colors.White;
            //titleBar.BackgroundColor = Colors.Green;
            //titleBar.ButtonForegroundColor = Colors.White;
            //titleBar.ButtonBackgroundColor = Colors.SeaGreen;
            //titleBar.ButtonHoverForegroundColor = Colors.White;
            //titleBar.ButtonHoverBackgroundColor = Colors.DarkSeaGreen;
            //titleBar.ButtonPressedForegroundColor = Colors.Gray;
            //titleBar.ButtonPressedBackgroundColor = Colors.LightGreen;

            //// Set inactive window colors
            //titleBar.InactiveForegroundColor = Colors.Gainsboro;
            //titleBar.InactiveBackgroundColor = Colors.SeaGreen;
            //titleBar.ButtonInactiveForegroundColor = Colors.Gainsboro;
            //titleBar.ButtonInactiveBackgroundColor = Colors.SeaGreen;
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;
        }


        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            //string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password.Trim();

            // 🔹 Basic validation// Empty check
            if (string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password))
            {
                ShowError("Please enter email and password.");

                //MessageBox.Show("Please enter email and password.");
                Log.Warning("Login attempt with empty email or password at {Time}", DateTime.Now);
                return;
            }

            if (!ValidationHelper.IsValidEmail(email))
            {
                ShowError("Invalid email format.");
                return;
            }

            try
            {
                var loginRequest = new LoginRequest
                {
                    UsernameOrEmail = email,
                    Password = password
                };

                Log.Information("User '{Username}' attempting to log in...", email);
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // 🔹 Unauthorized → user not found / wrong password
                    // User not registered or incorrect password
                    ShowError("Invalid email or password. Please register first.");
                    return;
                }

                // 🔹 Any other failure
                // 🚨 USER NOT REGISTERED//        // USER NOT FOUND (API returns 404)

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    ShowError("This email is not registered. Please register first.");
                    return;
                }
                // VALIDATION ERROR (API returns 400)
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    ShowError("Invalid login details.");
                    //var details = await response.Content.ReadAsStringAsync();
                    return;
                }

                // WRONG PASSWORD (API returns 401)
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ShowError("Incorrect password.");
                    return;
                }


                // 🚨 WRONG PASSWORD
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ShowError("Incorrect password. Please try again.");
                    return;
                }

                // 🚨 OTHER ERRORS
                if (!response.IsSuccessStatusCode)
                {
                    ShowError("Login failed. Try again later.");
                    return;
                }


                if (response.IsSuccessStatusCode)
                {
                    // 🎉 LOGIN SUCCESS

                    //Success → handle token & user session
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
                    {
                        ShowError("Invalid response from server.");
                        return;
                    }


                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                    {
                        //Log.Information("User '{Email}' logged in successfully as {Role}", email, loginResponse.Role);
                        Log.Information("User '{Email}' logged in successfully as {Role}", email, loginResponse.Role);

                        // 1️⃣ Save session token globally (in memory)
                        AppSession.JwtToken = loginResponse.Token;
                        AppSession.Role = loginResponse.Role;
                        AppSession.Username = loginResponse.Username;
                        AppSession.Email = email;
                        AppSession.UserId = loginResponse.UserId;

                        // 2. set token/session from API response OR 30 mins
                        //AppSession.ExpiryTime = DateTime.UtcNow.AddMinutes(60);
                        AppSession.ExpiryTime = DateTime.Now.AddHours(1);


                        // 3️⃣ Save login info locally ONLY IF Remember Me is checked for Automatic login (remember me)/✔ Offline login
                        if (RememberMeCheckBox.IsChecked == true)
                        {
                            Settings.Default.Username = loginResponse.Username;
                            Settings.Default.Role = loginResponse.Role;
                            Settings.Default.Email = email;
                            Settings.Default.JwtToken = loginResponse.Token;
                            Settings.Default.UserId = loginResponse.UserId.ToString();
                            Settings.Default.JwtToken = loginResponse.Token;
                            Settings.Default.ExpiryTime = AppSession.ExpiryTime;
                            Settings.Default.RememberMe = true;
                            Settings.Default.Save();
                        }
                        else
                        {
                            Settings.Default.RememberMe = false;

                            // Clear stored values if user did NOT choose remember me
                            //Settings.Default.Username = "";
                            //Settings.Default.Role = "";
                            //Settings.Default.Email = "";
                            //Settings.Default.UserId = "";
                            //Settings.Default.JwtToken = "";
                            //Settings.Default.Save();
                        }

                        // Clear previous errors
                        txtError.Text = string.Empty;

                        var toast = new SuccessToast($"Login successful! Welcome, {loginResponse.Username}");
                        ToastContainer.Children.Add(toast);

                        // Let UI refresh immediately
                        await Application.Current.Dispatcher.InvokeAsync(() => { }, System.Windows.Threading.DispatcherPriority.Background);

                        // Wait a little so animation shows
                        await Task.Delay(1000);


                        // ✅ Create service with injected token// After successful login
                        var client = new HttpClient
                        {
                            BaseAddress = new Uri("https://localhost:44357/")
                        };

                        var notesService = new NotesApiService(client);
                        await notesService.SetJwtToken(loginResponse.Token);
                        //var notesService = new NotesApiService(loginResponse.Token); //old code


                        AppSession.NotesService = notesService;


                        // ✅ Navigate/open to MainWindow with dependency injection after login and pass JWT if (loginResponse.Role == "Admin")
                        //var mainWindow = new MainWindow(notesService, loginResponse.Username, loginResponse.Role, email);
                        //mainWindow.Show();
                        var main = App._serviceProvider.GetRequiredService<MainWindow>();
                        main.Show();
                        this.Close();// ✅ Close Login window
                    }
                    else
                    {
                        Log.Warning("Login response invalid or missing token for user '{Username}'", email);
                        MessageBox.Show("Invalid response from server.");
                    }
                }
                else
                {
                    Log.Warning("Login failed for user '{Username}'. StatusCode: {StatusCode}", email, response.StatusCode);
                    MessageBox.Show($"Login failed: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                Log.Error(ex, "HTTP error during login attempt for user '{Username}'", email);

                if (ex.InnerException is System.Net.Sockets.SocketException socketEx)
                {
                    // API is offline or port is wrong
                    if (socketEx.SocketErrorCode == System.Net.Sockets.SocketError.ConnectionRefused)
                    {
                        ShowError("❌ Cannot connect to server. Please make sure the API is running.");
                        return;
                    }
                }

                // General network issues
                ShowError("🌐 Network issue detected. Please check your internet connection.");
                //Log.Error(ex, "Network error during login attempt for user '{Username}'", email);
                //ShowError("Network error. Please check your internet.");
            }
            catch (TaskCanceledException)
            {
                // Timeout
                ShowError("⏳ Server timeout. The API is taking too long to respond.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error during login attempt for user '{Username}'", email);
                ShowError("Unexpected error. Please try again.");
            }
        }

        private void Hyperlink_Click_Register(object sender, RoutedEventArgs e)
        {
            var registrationWindow = new RegistrationWindow();
            registrationWindow.ShowDialog(); // Use ShowDialog to make it modal
        }
    }
}
