using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AINotesHub.Shared;
using AINotesHub.WPF.Helpers;
using AINotesHub.WPF.Models;
using AINotesHub.WPF.Services;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace AINotesHub.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NotesApiService _notesService = new NotesApiService();
        public ObservableCollection<NoteViewModel> Notes { get; set; } = new ObservableCollection<NoteViewModel>();
        private List<Note> AllNotes = new List<Note>();
        //public ObservableCollection<Note> Notes { get; set; } = new ObservableCollection<Note1>();
        // Master list of all notes (never filtered)
        private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:44357/") };
        private NoteViewModel? _editing = null;
        private Brush _selectedColor = Brushes.White;
        public ICollectionView FilteredNotes { get; set; }
        private readonly TimeZoneInfo _indianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");


        public MainWindow()
        {
            InitializeComponent();
            SetDefaultTheme();
            //DataContext = this;
            LstNotes.ItemsSource = Notes;
            //CardColorVM = new NoteViewModel();
            //this.DataContext = CardColorVM;
            BtnAll.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#673AB7"));
            BtnAll.Foreground = Brushes.White;
            // Load default theme at startup
            ApplyTheme("OceanBlue");
            //Notes.Add(new Note { Title = "Shopping", Content = "Buy milk and eggs", ColorHex = "#FFF59D" });
            //Notes.Add(new Note { Title = "Meeting", Content = "Call with team at 3 PM", ColorHex = "#A5D6A7" });
            //Notes.Add(new Note { Title = "Ideas", Content = "App: AI note auto-summarize", ColorHex = "#90CAF9" });
            //icNotes.ItemsSource = Notes;

            // Example notes (in UTC)
            //Notes.Add(new Note { Title = "Weekly Meeting", CreatedAt = DateTime.UtcNow });
            //Notes.Add(new Note { Title = "Next Week Task", CreatedAt = DateTime.UtcNow.AddDays(6) });
            //Notes.Add(new Note { Title = "Next Month Plan", CreatedAt = DateTime.UtcNow.AddMonths(1) });
            //FilteredNotes = CollectionViewSource.GetDefaultView(Notes);
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
            await LoadNotesAsync();
        }
        private async Task LoadNotesAsync()
        {
            try
            {
                var notes = await _notesService.GetAllNotesAsync();
                Notes.Clear();
                foreach (var note in notes)
                {
                    //CardColorVM.CardColor = note.CardBackground;
                    // Create a NoteViewModel for each Note
                    var noteVM = new NoteViewModel(note);
                    //note.CardBackground= CardColorVM;
                    Notes.Add(noteVM);
                    AllNotes.Add(note);
                }
                

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load notes: {ex.Message}");
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
            BtnAdd.Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(accent));
        }

        //private void ApplyColors(string primary, string secondary, string background)
        //{
        //    // Example: apply to app resources or root grid background
        //    Application.Current.Resources["PrimaryColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(primary));
        //    Application.Current.Resources["SecondaryColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(secondary));
        //    Application.Current.Resources["BackgroundColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(background));
        //}

        //private void ColorButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sender is Button btn && btn.Tag is string colorCode)
        //    {
        //        var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorCode));
        //        // Apply wherever needed (background, card, etc.)
        //    }
        //}
        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (sender is Button btn && btn.Tag != null)
            {
                _selectedColor = (Brush)new BrushConverter().ConvertFromString(btn.Tag.ToString());
            }
        }
        // ===== Sidebar color clicked (select color for Apply-to-all) =====
        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var title = TxtTitle.Text.Trim();
            var content = TxtContent.Text.Trim();
            var Category = TxtCategory.Text.Trim();
            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(content))
            {
                MessageBox.Show("Please enter a title or content.");
                return;
            }

            //Category = TxtCategory.Text,
            //CreatedAt = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"),
            //CardBackground = _selectedColor

            if (_editing == null)
            {
                var note = new Note
                {
                    Title = title,
                    Content = content,
                    CreatedAt = DateTime.Now,
                    Category = Category,

                    //CreatedAt = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"),
                    CardBackground = _selectedColor?.ToString()

                };

                var result = await _notesService.AddNoteAsync(note);
                if (result.IsSuccess)
                {
                    Notes.Add(new NoteViewModel(note));

                    //Notes.Add(note);
                    AllNotes.Add(note);
                }
                else
                {
                    MessageBox.Show(result.Message);

                }

            }
            else
            {
                _editing.Title = title;
                _editing.Content = content;

                var result = await _notesService.UpdateNoteAsync(_editing.Model);

                if (result.IsSuccess)
                {
                    LstNotes.Items.Refresh();
                }
                else
                {
                    MessageBox.Show(result.Message);
                }
                _editing = null;
                BtnAdd.Content = "Add Note";
            }

            TxtTitle.Clear();
            TxtContent.Clear();
        }
        

        //private void BtnEdit_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sender is FrameworkElement fe && fe.Tag is Note note)
        //    {
        //        _editing = note;
        //        TxtTitle.Text = note.Title;
        //        TxtContent.Text = note.Content;
        //        BtnAdd.Content = "Update Note";
        //    }
        //}

        //private void BtnDelete_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sender is FrameworkElement fe && fe.Tag is Note note)
        //    {
        //        Notes.Remove(new NoteViewModel(note));
        //    }
        //}

        private void Card_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is MaterialDesignThemes.Wpf.Card card)
            {
                // Apply the selected color ONLY when adding
                //card.Background = _selectedColor;
            }
        }

        private void EditNote_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is NoteViewModel noteVm)
            {
                _editing = noteVm;
                TxtTitle.Text = noteVm.Title;
                TxtContent.Text = noteVm.Content;
                TxtCategory.Text = noteVm.Category;
                BtnAdd.Content = "Update Note";
            }
        }

        private void DeleteNote_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is NoteViewModel noteVm)
            {
                if (MessageBox.Show("Are you sure to delete this note?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    //Notes.Remove(note);
                    Notes.Remove(noteVm);
                    AllNotes.Remove(noteVm.Model);
                    //AllNotes.Remove(new NoteViewModel(noteVm));
                }
            }
        }

        private void CmbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private bool _isUpdating = false;
        private void FilterButton_Checked(object sender, RoutedEventArgs e)
        {
            if (_isUpdating) return;

            try
            {
                _isUpdating = true;

                // Prevent unchecking all buttons
                if (!(BtnAll.IsChecked == true || BtnToday.IsChecked == true || BtnThisWeek.IsChecked == true || BtnThisMonth.IsChecked == true))
                {
                    ((ToggleButton)sender).IsChecked = true;
                }


                // Reset all
                ResetButtonColors(BtnAll, BtnToday, BtnThisWeek, BtnThisMonth);

                // Highlight clicked
                var clickedButton = sender as ToggleButton;
                clickedButton.Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#673AB7"));
                clickedButton.Foreground = Brushes.White;

                string filter = clickedButton.Content.ToString();
                ApplyFilter(filter);
            }
            finally
            {
                _isUpdating = false;
            }
        }
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
                //// Prevent unchecking all buttons
                //if (!(BtnAll.IsChecked == true || BtnToday.IsChecked == true || BtnThisWeek.IsChecked == true || BtnThisMonth.IsChecked == true))
                //{
                //    ((ToggleButton)sender).IsChecked = true;
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "FilterButton_Unchecked", MessageBoxButton.OK, MessageBoxImage.Error);

                throw;
            }
            
        }
        private void ApplyFilter(string filter)
        {
            try
            {
                if (AllNotes == null || AllNotes.Count == 0)
                    return;

                IEnumerable<Note> filteredNotes = filter switch
                {
                    "All" => AllNotes, // ✅ Show all notes
                    "Today" => AllNotes.Where(n => DateFilterHelper.IsToday(n.CreatedAt)),
                    "This Week" => AllNotes.Where(n => DateFilterHelper.IsThisWeek(n.CreatedAt)),
                    "Next Week" => AllNotes.Where(n => DateFilterHelper.IsNextWeek(n.CreatedAt)),
                    "This Month" => AllNotes.Where(n => DateFilterHelper.IsThisMonth(n.CreatedAt)),
                    "Next Month" => AllNotes.Where(n => DateFilterHelper.IsNextMonth(n.CreatedAt)),
                    _ => AllNotes
                };

                Notes.Clear(); // Clear previous notes
                foreach (var note in filteredNotes)
                    //Notes.Add(note);
                    Notes.Add(new NoteViewModel(note));


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Filter error: {ex.Message}");
                //throw;
            }
        }
    }
}
