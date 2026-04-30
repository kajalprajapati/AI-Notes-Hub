using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using AINotesHub.Shared.Entities;
using AINotesHub.WPF.Helpers;
using AINotesHub.WPF.Services;
using AINotesHub.WPF.UserControls;
using AINotesHub.WPF.UserControls.Dialogs;
using AINotesHub.WPF.ViewModels;
using AINotesHub.WPF.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;


namespace AINotesHub.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _role;
        private bool _isUpdating = false;
        private readonly NotesApiService _notesService;
        // Master list of all notes (never filtered)
        //private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:44357/") };
        private NoteViewModel? _editing = null;
        private readonly TimeZoneInfo _indianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private MainViewModel _mainVm;
        //public MainWindow(NotesApiService notesService, string username, string role, string email)
        public MainWindow(NotesApiService notesService, MainViewModel mainVm)

        {
            InitializeComponent();
            _notesService = notesService;
            AppSession.NotesService = notesService;

            if (!string.IsNullOrEmpty(AppSession.JwtToken))
            {
                _notesService.SetJwtToken(AppSession.JwtToken);
                //username = username;
                //_role = role;

            }

            //var mainVm = App._serviceProvider.GetRequiredService<MainViewModel>();
            mainVm.CurrentUser = new UserViewModel
            {
                Username = mainVm.Username,
                Email = mainVm.Email,
                Role = mainVm.Role
            };

            DataContext = mainVm;

            SetDefaultTheme();


            foreach (var note in mainVm.Notes)
            {
                note.OpenRequested += OnOpenNoteRequested;
            }
            //LstNotes.ItemsSource = Notes;


            //BtnAll.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2C58B6"));
            //BtnAll.Foreground = Brushes.White;


            ToastService.OnShowSuccess += ShowToast;

        }
        private void ShowToast(string msg)
        {
            Dispatcher.Invoke(() =>
            {
                ToastContainer.Children.Add(new SuccessToast(msg));
            });
        }
        private async void OnOpenNoteRequested(NoteViewModel noteVm)
        {
            try
            {
                if (AppSession.NotesService == null)
                {
                    MessageBox.Show("Session expired. Please login again.");
                    return;
                }

                var mainVm = DataContext as MainViewModel;
                if (mainVm == null) return;

                var dialog = new NoteDetailsDialog(
                    AppSession.NotesService,
                    noteVm,
                    mainVm, AppSession.ColorState);

                await DialogHost.Show(dialog, "RootDialogHost");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SetDefaultTheme()
        {
            // Default theme
            string primary = "#FFFFFFFF";   // Main window background (white)
            string secondary = "#F6F8FA";   // Notes container background (light gray)
            string accent = "#FF1976D2";    // Buttons, highlights (blue)

            ApplyColors(primary, secondary, accent);
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //                MessageBox.Show(
                //    "ExpiryTime Loaded: " + Settings.Default.ExpiryTime +
                //    "\nNow: " + DateTime.Now
                //);
                //Loaded += (s, e) => Debug.WriteLine(DialogHost.IsDialogOpen("RootDialogHost"));
                if (AppSession.IsExpired)
                {

                    //await SessionHelper.ClearSession("Session Expired");
                    await AppSession.Clear();
                    //MessageBox.Show("Session expired. Please login again.");
                    MessageBox.Show(
        "Your session has expired. Please login again.",
        "Session Expired",
        MessageBoxButton.OK,
        MessageBoxImage.Warning);
                    //ProfileImage.Visibility = Visibility.Collapsed;
                    //DefaultAvatar.Visibility = Visibility.Visible;

                    // Open Login Window
                    var loginWindow = new LoginWindow();
                    loginWindow.Show();

                    // Close current window
                    Application.Current.MainWindow.Close();
                    return;
                }
                else
                {
                    Log.Information("MainWindow loaded for user {Username} with role {Role}",
                    AppSession.Username, AppSession.Role);

                    // Create and assign your main view model instance
                    //var mainVm = new MainViewModel(_notesService);//Insted of this use DI

                    var vm = App._serviceProvider.GetRequiredService<MainViewModel>();
                    //DataContext = App._serviceProvider.GetRequiredService<MainViewModel>();
                    Debug.WriteLine($"UI VM: {vm.GetHashCode()}");
                    DataContext = vm;

                    if (DataContext != null)
                    {
                        

                        // Get filtered notes from somewhere (your data source or API)
                        var filteredNotes = await AppSession.NotesService.GetNotesAsync(); // example async call to fetch notes
                                                                                           // Call LoadNotesAsync with required arguments
                        if (DataContext is MainViewModel mvm)
                        {
                            await mvm.LoadNotesAsync();
                        }

                        //await mainVm.LoadNotesAsync();    

                    }
                    //}
                    //await LoadNotesAsync(filteredNotes, AppSession.NotesService, mainVm);
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred during MainWindow load.");
            }
        }
        private void ThemePalette_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is string themeName)
            {
                ApplyTheme(themeName);
            }
        }
        private void ApplyTheme(string themeName)
        {
            // Each theme can apply coordinated colors (e.g., background, accent, text)
            string primary = "#FF1976D2";
            string secondary = "#FFBBDEFB";
            string accent = "#FF64B5F6";

            switch (themeName)
            {
                case "OceanBlue":
                    primary = "#FF1976D2";
                    secondary = "#FFBBDEFB";
                    accent = "#FF64B5F6";
                    break;
                case "SunsetGlow":
                    primary = "#FFFF7043";
                    secondary = "#FFFFE082";
                    accent = "#FFFFB74D";
                    break;
                case "MintFresh":
                    primary = "#FF4DB6AC";
                    secondary = "#FFB2DFDB";
                    accent = "#FF80CBC4";
                    break;
                    // Add other themes
            }

            // Update DynamicResources
            this.Resources["ThemePrimaryBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(primary));
            this.Resources["ThemeSecondaryBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(secondary));
            this.Resources["ThemeAccentBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(accent));

        }
        private void ApplyColors(string primary, string secondary, string accent)
        {
            // Main Grid or Window background
            this.Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(primary));

            // Background for each note card can be default secondary if not set per note
            foreach (var item in LstNotes.Items)
            {
                if (LstNotes.ItemContainerGenerator.ContainerFromItem(item) is FrameworkElement container)
                {
                    if (container is ContentPresenter presenter &&
                        presenter.ContentTemplate.FindName("CardRoot", presenter) is MaterialDesignThemes.Wpf.Card card)
                    {
                        card.Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(secondary));
                    }
                }
            }

            // Accent buttons
            //BtnAdd.Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(accent));
        }
        private void Card_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is MaterialDesignThemes.Wpf.Card card)
            {
                // Apply the selected color ONLY when adding
                //card.Background = _selectedColor;
            }
        }
        //private void EditNote_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sender is Button btn && btn.Tag is NoteViewModel noteVm)
        //    {
        //        _editing = noteVm;
        //        TxtTitle.Text = noteVm.Title;
        //        TxtContent.Text = noteVm.Content;
        //        TxtCategory.Text = noteVm.Category;
        //        //BtnAdd.Content = "Update Note";
        //    }
        //}
        //private async void DeleteNote_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sender is Button btn && btn.Tag is NoteViewModel noteVm)
        //    {
        //        // ✅ Call API

        //        if (MessageBox.Show("Are you sure to delete this note?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        //        {
        //            try
        //            {
        //                var result = await _notesService.DeleteNoteAsync(noteVm.Model.Id);

        //                if (result.IsSuccess)
        //                {
        //                    //Notes.Remove(note);
        //                    Notes.Remove(noteVm);
        //                    AllNotes.Remove(noteVm.Model);
        //                    var toast = new SuccessToast("Note deleted successfully.");
        //                    ToastContainer.Children.Add(toast);
        //                    await Application.Current.Dispatcher.InvokeAsync(() => { }, System.Windows.Threading.DispatcherPriority.Background);
        //                    await Task.Delay(5000);// Wait a little so animation shows
        //                    Log.Warning("User {Username} deleted note '{Title}' (ID: {NoteId})",
        //                        AppSession.Username, noteVm.Title, noteVm.Model.Id);


        //                }
        //                else
        //                {
        //                    Log.Error("Failed to delete note '{Title}' for user {Username}: {Message}",
        //                        noteVm.Title, AppSession.Username, result.Message);
        //                    MessageBox.Show(result.Message);
        //                }


        //            }
        //            catch (Exception ex)
        //            {
        //                Log.Error(ex, "Error deleting note '{Title}' (ID: {NoteId}) by user {Username}",
        //                    noteVm.Title, noteVm.Model.Id, AppSession.Username);
        //            }
        //        }
        //    }
        //}
        private void CmbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        //private void FilterButton_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (_isUpdating) return;

        //    try
        //    {
        //        _isUpdating = true;

        //        // Prevent unchecking all buttons
        //        if (!(BtnAll.IsChecked == true || BtnToday.IsChecked == true || BtnThisWeek.IsChecked == true || BtnThisMonth.IsChecked == true))
        //        {
        //            ((ToggleButton)sender).IsChecked = true;
        //        }


        //        // Reset all
        //        ResetButtonColors(BtnAll, BtnToday, BtnThisWeek, BtnThisMonth);

        //        // Highlight clicked
        //        var clickedButton = sender as ToggleButton;
        //        clickedButton.Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2C58B6"));
        //        clickedButton.Foreground = Brushes.White;

        //        string filter = clickedButton.Content.ToString();
        //        ApplyDateFilter(filter);
        //    }
        //    finally
        //    {
        //        _isUpdating = false;
        //    }
        //}
        private void ResetButtonColors(params ToggleButton[] buttons)
        {
            var defaultBackground = Brushes.Transparent;
            var defaultForeground = Brushes.Gray;

            foreach (var btn in buttons)
            {
                if (btn != null)
                {
                    btn.Background = defaultBackground;
                    btn.Foreground = defaultForeground;
                }
            }
        }
        private void FilterButton_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var clickedButton = sender as ToggleButton;

                // Get parent StackPanel (or container)
                var parentPanel = VisualTreeHelper.GetParent(clickedButton) as Panel;
                if (parentPanel == null) return;

                // Find all ToggleButtons in the same panel
                var toggleButtons = parentPanel.Children.OfType<ToggleButton>();

                // If all are unchecked, recheck the one the user clicked
                if (!toggleButtons.Any(b => b.IsChecked == true))
                {
                    clickedButton.IsChecked = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "FilterButton_Unchecked", MessageBoxButton.OK, MessageBoxImage.Error);

                throw;
            }

        }

        //private void ApplyDateFilter(string filterKey)
        //{
        //    try
        //    {
        //        if (AllNotes == null || AllNotes.Count == 0)
        //            return;

        //        IEnumerable<Note> filteredNotes = filterKey switch
        //        {
        //            "All" => AllNotes, // ✅ Show all notes
        //            "Today" => AllNotes.Where(n => DateFilterHelper.IsToday(n.CreatedAt)),
        //            "This Week" => AllNotes.Where(n => DateFilterHelper.IsThisWeek(n.CreatedAt)),
        //            "Next Week" => AllNotes.Where(n => DateFilterHelper.IsNextWeek(n.CreatedAt)),
        //            "This Month" => AllNotes.Where(n => DateFilterHelper.IsThisMonth(n.CreatedAt)),
        //            "Next Month" => AllNotes.Where(n => DateFilterHelper.IsNextMonth(n.CreatedAt)),
        //            _ => AllNotes
        //        };

        //        Notes.Clear(); // Clear previous notes

        //        var mainVm = this.DataContext as MainViewModel;

        //        if (mainVm == null)
        //        {
        //            MessageBox.Show("MainViewModel is not set.");
        //            return;
        //        }

        //        //var mainVm = DataContext as MainViewModel;
        //        foreach (var note in filteredNotes)
        //            //Notes.Add(note);
        //            //Notes.Add(new NoteViewModel(note, AppSession.NotesService, mainVm));// MainViewModel)
        //            Notes.Add(App.Services.GetRequiredService<NoteViewModel>());

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Filter error: {ex.Message}");
        //        //throw;
        //    }
        //}

    }
}
