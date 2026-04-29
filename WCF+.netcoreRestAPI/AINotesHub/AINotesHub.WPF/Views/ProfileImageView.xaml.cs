using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AINotesHub.WPF.Helpers;
using AINotesHub.WPF.ViewModels;
using Serilog;

namespace AINotesHub.WPF.Views
{
    /// <summary>
    /// Interaction logic for ProfileImageView.xaml
    /// </summary>
    public partial class ProfileImageView : UserControl
    {
        public ProfileImageView()
        {
            InitializeComponent();
        }
        //public ProfileImageView(UserViewModel userVm) : this()
        //{
        //    InitializeComponent();

        //    DataContext = userVm;

        //    string savedImage = Properties.Settings.Default.ProfileImagePath;

        //    if (!string.IsNullOrEmpty(savedImage) && File.Exists(savedImage))
        //    {
        //        ProfileImage.Source = new BitmapImage(new Uri(savedImage));

        //        //ProfileImage.Source = LoadBitmap(saved);
        //        ProfileImage.Visibility = Visibility.Visible;
        //        DefaultAvatar.Visibility = Visibility.Collapsed;
        //    }

        //    else
        //    {
        //        ProfileImage.Visibility = Visibility.Collapsed;
        //        DefaultAvatar.Visibility = Visibility.Visible;
        //    }


           

        //}

        public void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to logout?", "Confirm Logout",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    // 🧹 Clear in-memory session
                    AppSession.Username = string.Empty;
                    AppSession.Role = string.Empty;
                    AppSession.UserId = Guid.Empty;
                    AppSession.JwtToken = string.Empty;

                    // 🧹 Clear persisted settings
                    Properties.Settings.Default.Username = string.Empty;
                    Properties.Settings.Default.Role = string.Empty;
                    Properties.Settings.Default.UserId = string.Empty;
                    Properties.Settings.Default.JwtToken = string.Empty;
                    Properties.Settings.Default.Save();

                    Log.Information("User {Username} logged out successfully.", AppSession.Username);

                    // 🪟 Navigate back to login screen
                    var loginWindow = new LoginWindow();
                    loginWindow.Show();

                    // Fix: Close the parent window containing this UserControl
                    Window parentWindow = Window.GetWindow(this);
                    if (parentWindow != null)
                    {
                        parentWindow.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred during logout process.");

                MessageBox.Show("Something went wrong while logging out. Please try again.",
                    "Logout Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ChangeProfile_Click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog dlg = new OpenFileDialog();
            //dlg.Title = "Select Profile Picture";
            ////dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"; 
            //dlg.Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg";
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg";

            if (dialog.ShowDialog() == true)
            {
                try
                {

                    //Guid identity = AppSession.UserId; // use email, id, GUID

                    string identity = AppSession.UserId.ToString(); // use email, id, GUID
                    // Open document
                    string filename = dialog.FileName;
                    string selectedFile = dialog.FileName;

                    // Create secure hashed folder for the user
                    string baseFolder = System.IO.Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "MyApp",
                        "Profiles");


                    if (!Directory.Exists(baseFolder))
                        Directory.CreateDirectory(baseFolder);


                    // SECURITY: Hash identity (never expose username/email)
                    string hash = HashIdentity(identity);
                    string userFolder = System.IO.Path.Combine(baseFolder, hash);

                    Directory.CreateDirectory(userFolder);

                    // Unique filename always
                    string ext = System.IO.Path.GetExtension(selectedFile);
                    string uniqueFileName = Guid.NewGuid().ToString("N") + ext;

                    string destinationFile = System.IO.Path.Combine(userFolder, uniqueFileName);

                    //// copy file with username
                    //string destinationFile = System.IO.Path.Combine(appFolder, $"{AppSession.Username}.png");

                    File.Copy(selectedFile, destinationFile, true);
                    ProfileImage.Visibility = Visibility.Visible;
                    DefaultAvatar.Visibility = Visibility.Collapsed;

                    // update image UI
                    ProfileImage.Source = new BitmapImage(new Uri(destinationFile));
                    //ProfileImage.Source = bitmap;


                    // save to settings
                    Properties.Settings.Default.ProfileImagePath = destinationFile;
                    Properties.Settings.Default.Save();


                    //// OPTIONAL: Save to local folder  
                    //string appFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyApp");
                    //Directory.CreateDirectory(appFolder);

                    //string savePath = System.IO.Path.Combine(appFolder, "profile.jpg");

                    //File.Copy(dlg.FileName, savePath, true);

                    MessageBox.Show("Profile picture updated successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
        }

        public static string HashIdentity(string input)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        public void ProfileMenuButton_Click(object sender, RoutedEventArgs e)
        {
            ProfilePopup.IsOpen = true;

        }



    }
}
