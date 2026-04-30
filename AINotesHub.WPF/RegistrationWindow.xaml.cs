using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AINotesHub.Shared;
using AINotesHub.Shared.DTOs;
using AINotesHub.WPF.Helpers;
using Serilog;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace AINotesHub.WPF
{
    /// <summary>
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:44357/") // ← Change to your API port
        };
        public RegistrationWindow()
        {
            InitializeComponent();
            //if (string.IsNullOrEmpty(_apiBaseUrl))
            //{
            //    // This check should already be in LoginWindow, but good to have here too
            //    MessageBox.Show("API Base URL not found in App.config.", "Configuration Error");
            //    Close();
            //    return;
            //}

            //_httpClient = new HttpClient
            //{
            //    BaseAddress = new Uri(_apiBaseUrl)
            //};
        }

        private async void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            // 1. Clear old /previous errors
            txtError.Text = "";

            // 2. Get data from UI
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            // 3. UI validation
            if (password != confirmPassword)
            {
                txtError.Text = "Passwords do not match.";
                return;
            }

            //if (!ValidationHelper.IsValidEmail(email))
            //{
            //    ShowError("Invalid email format.");
            //    return;
            //}

            // 4. Create the request model
            var registerRequest = new RegisterRequest
            {
                Username = username,
                Email = email,
                Password = password // The API only needs the password once
            };

            // 5. ✅ THIS IS THE MODEL VALIDATION
            var validationContext = new ValidationContext(registerRequest);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(registerRequest, validationContext, validationResults, true);

            if (!isValid)
            {
                // Collate all validation errors and show them
                var errorMessages = validationResults.Select(r => r.ErrorMessage);
                txtError.Text = string.Join("\n", errorMessages);
                MessageBox.Show(txtError.Text);
                return;
            }

            // 6. If valid, send to API/Call API
            btnRegister.IsEnabled = false; // Disable button during request
            try
            {
                Log.Information("Registering new user {Email}'...", email);

                // Assume your API has an "api/auth/register" endpoint
                var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerRequest);

                if (response.IsSuccessStatusCode)
                {
                    Log.Information("User '{email}' registered successfully.", email);
                    MessageBox.Show("Registration successful! You can now log in.", "Success");
                    this.Close(); // Close registration window
                    return;
                }
                else
                {
                    // Try to read the error message from the API
                    string apiError = await response.Content.ReadAsStringAsync();
                    string errorMessage = $"Registration failed ({(int)response.StatusCode})";

                    if (!string.IsNullOrEmpty(apiError))
                    {
                        // This handles errors like "Username already taken" if your API sends them
                        errorMessage += $": {apiError}";
                    }

                    Log.Warning("Registration failed for '{Username}': {Error}", email, errorMessage);
                    txtError.Text = errorMessage;
                }
            }
            catch (HttpRequestException ex)
            {
                Log.Error(ex, "Network error during registration for user '{Email}'", email);
                txtError.Text = $"Network error: {ex.Message}";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error during registration for user '{Email}'", email);
                txtError.Text = $"Unexpected error: {ex.Message}";
            }
            finally
            {
                btnRegister.IsEnabled = true; // Re-enable button
            }
        }

        

        private void CheckPasswordMatch()
        {
            string pwd = txtPassword.Password ?? string.Empty;
            string confirm = txtConfirmPassword.Password ?? string.Empty;

            if (string.IsNullOrEmpty(pwd) && string.IsNullOrEmpty(confirm))
            {
                txtError.Visibility = Visibility.Collapsed;
                txtError.Text = string.Empty;
                return;
            }

            if (pwd != confirm)
            {
                txtError.Text = "Passwords do not match.";
                txtError.Visibility = Visibility.Visible;
            }
            else
            {
                txtError.Visibility = Visibility.Collapsed;
                txtError.Text = string.Empty;
            }
        }

        private void txtConfirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            //if (txtConfirmPassword.Password != txtPassword.Password)
            //{
            //    txtError.Text = "Passwords do not match!";
            //}
            //else
            //{
            //    txtError.Text = "";
            //}
            CheckPasswordMatch();


        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string pwd = txtPassword.Password;
            int strength = ValidationHelper.CalculatePasswordStrength(pwd);

            pbStrength.Value = strength;

            txtStrengthText.Visibility =
                string.IsNullOrWhiteSpace(pwd) ? Visibility.Collapsed : Visibility.Visible;

            switch (strength)
            {
                case 0:
                case 1:
                    txtStrengthText.Text = "Weak";
                    txtStrengthText.Foreground = Brushes.Red;
                    break;
                case 2:
                    txtStrengthText.Text = "Medium";
                    txtStrengthText.Foreground = Brushes.Orange;
                    break;
                case 3:
                    txtStrengthText.Text = "Strong";
                    txtStrengthText.Foreground = Brushes.Green;
                    break;
                case 4:
                    txtStrengthText.Text = "Very Strong";
                    txtStrengthText.Foreground = Brushes.DarkGreen;
                    break;
            }
        }
    }
}
