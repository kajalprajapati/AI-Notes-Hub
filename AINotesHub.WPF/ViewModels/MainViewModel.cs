using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using AINotesHub.API.Services;
using AINotesHub.WPF.Enums;
using AINotesHub.WPF.Factories;
using AINotesHub.WPF.Helpers;
using AINotesHub.WPF.Services;
using AINotesHub.WPF.UserControls.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MaterialDesignThemes.Wpf;
using Serilog;
using Windows.Media.Protection.PlayReady;
using Note = AINotesHub.Shared.Entities.Note;
using RelayCommand = CommunityToolkit.Mvvm.Input.RelayCommand;


namespace AINotesHub.WPF.ViewModels
{
    //public enum ViewType
    //{
    //    Calendar,
    //    ActiveNotes,
    //    Archive,
    //    Trash
    //}
    public class NavigationItem
    {
        public string Name { get; set; }
        public PackIconKind Icon { get; set; }
        public NoteViewType Mode { get; set; }
    }
    public partial class MainViewModel : ObservableObject
    {

        //private List<NoteViewType> _allNotes;
        private List<Note> _allNotes;
        private readonly NotesApiService _notesService;
        private readonly INoteColorService _colorService;
        private readonly NoteViewModelFactory _noteVmFactory;
        private readonly ColorStateViewModel _colorState;
        private readonly IServiceProvider _serviceProvider;
        private readonly DapperService _dapperService; // Dapper
        private NoteViewModel _editing;
        public ICollectionView FilteredNotes { get; }
        public ObservableCollection<NavigationItem> NavigationItems { get; }
        public event Action<string>? NoteDeletedSuccessfully;
        public ObservableCollection<NoteViewModel> Notes { get; set; } = new ObservableCollection<NoteViewModel>();
        public ObservableCollection<NoteViewModel> CurrentNotes { get; } = new();
        [ObservableProperty] private string _currentViewTitle = "My Notes"; // Optional: To show "Archive" in the header
        [ObservableProperty] private string _noteTitle;
        [ObservableProperty] private string _noteContent;
        [ObservableProperty] private string _noteCategory;
        [ObservableProperty] private string _addButtonText = "Add Note";
        [ObservableProperty] private string _selectedColorCode = "FFFFFF";
        [ObservableProperty] private NoteViewType _currentMode = NoteViewType.Active;
        [ObservableProperty] private UserViewModel _currentUser;
        [ObservableProperty] private bool _isLoadingNotes = true;
        [ObservableProperty] private NoteViewType _currentView;
        [ObservableProperty] private string _searchText = string.Empty;
        [ObservableProperty] private string _selectedSearchOption = "Contains";
        [ObservableProperty] private bool isMenuVisible = true;
        [ObservableProperty] private bool _isAddNoteDialogOpen;
        [ObservableProperty] private bool isSelected;
        private CancellationTokenSource _cts;
        public record SuccessMessage(string Message);
        private bool _isAddPopupOpen;
        public SidebarViewModel SidebarVM { get; }
        public object CurrentContent { get; set; }
        #region Commands
        public ICommand OpenAddNotePopupCommand { get; }
        public ICommand CloseAddNoteDialogCommand { get; }
        public ICommand ChangeColorCommand { get; }
        public ICommand OpenNoteCommand { get; }

        #endregion

        public Brush CurrentNoteColor => _colorState.CurrentNoteColor;
        public List<string> SearchOptions { get; } =
            new()
            {
                "Contains",
                "Exact Match",
                "Match Case"
            };

        #region EventDelegate
        partial void OnSelectedSearchOptionChanged(string value)
        {
            ApplySearchTextFilter();
        }

        private async Task DebouncedSearch()
        {
            _cts?.Cancel(); // cancel previous typing
            _cts = new CancellationTokenSource();

            try
            {
                await Task.Delay(500, _cts.Token); // wait 500ms
                await ApplySearchTextFilter();
            }
            catch (TaskCanceledException)
            {
                // user typed again → ignore
            }
        }
        private void OnColorChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(CurrentNoteColor));
        }
        partial void OnSearchTextChanged(string value)
        {
            _ = HandleSearchAsync();
        }

        private async Task HandleSearchAsync()
        {
            try
            {
                await DebouncedSearch();
            }
            catch (Exception ex)
            {
                // log error
            }
        }

        #endregion

        private async Task ApplySearchTextFilter()
        {
            try
            {
                //IsLoading = true;
                //HasError = false;

                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    // Load EF data
                    Notes = new ObservableCollection<NoteViewModel>(
                        _allNotes.Select(n => new NoteViewModel(n))
                    );
                    return;
                }

                Log.Information("Searching notes: {Keyword}", SearchText);

                var response = await _dapperService.SearchNotes(SearchText);

                if (response != null && response.Any())
                {
                    Notes = new ObservableCollection<NoteViewModel>(
                        response.Select(n => new NoteViewModel(n))
                    );
                }
                else
                {
                    Notes.Clear();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during search");

                //HasError = true;
                ErrorMessage = "Something went wrong";
            }
            finally
            {
                // IsLoading = false;
            }
        }
        private async Task ApplySearchTextFilterold()
        {
            try
            {
                Notes.Clear();

                if (string.IsNullOrWhiteSpace(SearchText))
                {


                    foreach (var note in _allNotes)
                        Notes.Add(new NoteViewModel(note));

                    return;
                }


                Func<string, string, bool> comparison = SelectedSearchOption switch
                {
                    "Exact Match" => (source, search) =>
                        string.Equals(source, search, StringComparison.OrdinalIgnoreCase),

                    "Match Case" => (source, search) =>
                        source.Contains(search, StringComparison.Ordinal),

                    _ => (source, search) =>
                        source.Contains(search, StringComparison.OrdinalIgnoreCase)
                };

                var filtered = _allNotes.Where(n =>
        comparison(n.Title, SearchText) ||
        comparison(n.Content, SearchText));

                Notes.Clear();

                foreach (var note in filtered)
                    Notes.Add(new NoteViewModel(note));
            }
            catch (Exception ex)
            {

                //throw;
            }
            finally
            {
                //IsLoading = false;
            }

        }
        private void ApplyCurrentFilter()
        {
            // 1. Start with the Master List
            IEnumerable<Note> filtered = CurrentMode switch
            {
                NoteViewType.Archived => _allNotes.Where(n => n.IsArchived && !n.IsDeleted),
                NoteViewType.Trash => _allNotes.Where(n => n.IsDeleted),
                NoteViewType.Calendar => _allNotes, // Or your specific calendar logic
                _ => _allNotes.Where(n => !n.IsArchived && !n.IsDeleted) // Default: Active
            };


            // 3. Update the UI Collection
            CurrentNotes.Clear();
            foreach (var note in filtered)
            {
                CurrentNotes.Add(new NoteViewModel(note));
            }

        }
        //private void ApplyDateFilter(NoteFilterType filter)
        //{
        //    try
        //    {
        //        if (_allNotes == null || !_allNotes.Any())
        //            return;

        //        IEnumerable<Note> filtered = filter switch
        //        {
        //            NoteFilterType.All => _allNotes,
        //            NoteFilterType.Today => _allNotes.Where(n => DateFilterHelper.IsToday(n.CreatedAt)),
        //            NoteFilterType.ThisWeek => _allNotes.Where(n => DateFilterHelper.IsThisWeek(n.CreatedAt)),
        //            NoteFilterType.NextWeek => _allNotes.Where(n => DateFilterHelper.IsNextWeek(n.CreatedAt)),
        //            NoteFilterType.ThisMonth => _allNotes.Where(n => DateFilterHelper.IsThisMonth(n.CreatedAt)),
        //            NoteFilterType.NextMonth => _allNotes.Where(n => DateFilterHelper.IsNextMonth(n.CreatedAt)),
        //            _ => _allNotes
        //        };

        //        Notes.Clear();

        //        foreach (var note in filtered)
        //        {
        //            Notes.Add(new NoteViewModel(note));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorMessage = $"Filter error: {ex.Message}";
        //    }
        //}

        public string Username { get; }
        public string Role { get; }
        public string Email { get; }
        public MainViewModel(NoteViewModelFactory noteVmFactory
            , ColorStateViewModel colorState,
            NotesApiService notesService,
            SidebarViewModel sidebarVM,
            ProfileImageViewModel profileVM,
            IServiceProvider serviceProvider /*, other dependencies*/)

        {

            Username = Properties.Settings.Default.Username;
            Role = Properties.Settings.Default.Role;
            Email = Properties.Settings.Default.Email;

            _noteVmFactory = noteVmFactory
        ?? throw new ArgumentNullException(nameof(noteVmFactory));

            _notesService = notesService
                ?? throw new ArgumentNullException(nameof(notesService));

            _colorState = colorState
        ?? throw new ArgumentNullException(nameof(colorState));

        //    _dapperService = dapperService
        //?? throw new ArgumentNullException(nameof(dapperService));

            SidebarVM = sidebarVM;
            _serviceProvider = serviceProvider;
            //CurrentView = profileVM;
            CurrentContent = profileVM ?? throw new ArgumentNullException(nameof(profileVM));
            // 👇 ADD IT HERE
            //CurrentContent = _serviceProvider
            //    .GetRequiredService<ProfileImageViewModel>();
            Debug.WriteLine(CurrentContent.GetType().Name);

            _colorState.PropertyChanged += ColorState_PropertyChanged;

            _allNotes = new List<Note>();

            // internal cache
            CurrentView = NoteViewType.ActiveNotes;

            OpenAddNotePopupCommand = new RelayCommand(OpenAddNotePopup);
            CloseAddNoteDialogCommand = new RelayCommand(CloseAddNoteDialog);
            OpenNoteCommand = new AsyncRelayCommand<NoteViewModel>(OpenNoteAsync);

            //Notes.CollectionChanged += (s, e) =>
            //{
            //    OnPropertyChanged(nameof(HasNotes));
            //};
        }
        private async Task OpenNoteAsync(NoteViewModel? noteVm)
        {
            if (noteVm == null)
                return;

            try
            {
                if (AppSession.NotesService == null)
                {
                    MessageBox.Show("Session expired. Please login again.");
                    return;
                }

                var detailView = new NoteDetailsDialog(
                    AppSession.NotesService,
                    noteVm,
                    this,
                    AppSession.ColorState);

                var result = await DialogHost.Show(detailView, "RootDialogHost");

                if (result?.ToString() == "Deleted")
                {
                    // Refresh notes if needed
                    // LoadNotes();
                }
            }
            catch (Exception ex)
            {
                Log.Error(
                    ex,
                    "Error opening note details '{Title}' (ID: {NoteId}) by user {Username}",
                    noteVm.Title,
                    noteVm.Model?.Id,
                    AppSession.Username
                );

                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Menu()
        {
            IsMenuVisible = !IsMenuVisible;
        }
        private void ColorState_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ColorStateViewModel.CurrentNoteColor))
            {
                OnPropertyChanged(nameof(CurrentNoteColor));
                Debug.WriteLine("MainVM notified");
            }
        }
        private void ShowActiveNotes()
        {
            CurrentViewTitle = "My Notes";
            CurrentMode = NoteViewType.Active; // Set the mode
            ApplyCurrentFilter();              // Run the shared filter logic
        }
        [RelayCommand]
        private void ShowArchive()
        {
            CurrentViewTitle = "Archived Notes";
            CurrentMode = NoteViewType.Archived;
            ApplyCurrentFilter();
        }

        [RelayCommand]
        private void ShowTrash()
        {
            CurrentViewTitle = "Trash";
            CurrentMode = NoteViewType.Trash;
            ApplyCurrentFilter();
        }

        [RelayCommand]
        public void Show_allNotes()
        {
            CurrentViewTitle = "My Notes";

            // Show only notes that are NOT archived and NOT deleted
            var active = _allNotes.Where(n => !n.IsArchived && !n.IsDeleted).ToList();

            CurrentNotes.Clear();
            foreach (var note in active)
            {
                CurrentNotes.Add(new NoteViewModel(note));
            }
        }

        // Simple message class

        [ObservableProperty]
        private string? errorMessage;

        private static int _counter = 1;
        //public async Task<string> GenerateDefaultTitleAsync(Guid userId)
        //{
        //    var nextNumber = await _dapperService.GetNextUntitledNumber(userId);

        //    return $"Untitled {nextNumber}";
        //    //return $"Untitled {_counter++}";
        //}

        
        private bool IsInvalidTitle(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return true;

            return text.StartsWith("Error", StringComparison.OrdinalIgnoreCase)
                || text.Contains("Unable to reach", StringComparison.OrdinalIgnoreCase)
                || text.Contains("timed out", StringComparison.OrdinalIgnoreCase)
                || text.Contains("failed", StringComparison.OrdinalIgnoreCase);
        }

        [RelayCommand]
        private async Task AddNoteAsync()
        {
            var title = NoteTitle?.Trim();
            var content = NoteContent?.Trim();
            var category = NoteCategory?.Trim();
            //IsAddPopupOpen = false;   // 🔥 POPUP CLOSES
            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(content))
            {
                Log.Warning("Empty note add attempt by user {Username}", AppSession.Username);
                MessageBox.Show("Please enter a title or content.");
                return;
            }

            try
            {
                if (_editing == null) // ADD MODE
                {
                    //if (string.IsNullOrWhiteSpace(title))
                    //{
                    var aiTitle = await _notesService.GenerateTitle(content);
                    //}
                    //    title = await _noteService.GenerateDefaultTitleAsync(AppSession.UserId);

                    if (AppSession.UserId == Guid.Empty)
                    {
                        throw new Exception("User not logged in properly");
                    }
                    if (IsInvalidTitle(aiTitle))
                    {
                        var nextNumber = await _notesService.GetNextUntitledNumberFromApi(AppSession.UserId);
                        title = $"Untitled {nextNumber}";
                    }
                    else
                    {
                        title = aiTitle.Trim().Replace("\"", "");
                    }
                    // 🔥 handle error / empty / bad response
                    //title = IsInvalidTitle(aiTitle)
                    //    ? await GenerateDefaultTitleAsync(AppSession.UserId)
                    //    : aiTitle.Trim();

                    //if (title.Contains("Error:"))
                    //{
                    //    title = $"United " + NoteTitleNum;
                    //}
                    //title = "Untitled 3";
                    var note = new Note
                    {
                        Title = title,
                        Content = content,
                        CreatedAt = DateTime.Now,
                        Category = category,
                        CardBackground = CurrentNoteColor?.ToString(),
                        CreatedBy = AppSession.Username ?? "Unknown",
                        UserId = AppSession.UserId

                    };

                    var result = await _notesService.AddNoteAsync(note);
                    if (result.IsSuccess)
                    {
                        //var noteVm = new NoteViewType(note, _notesService, this, _colorState);
                        //var noteVm = App.Services.GetRequiredService<NoteViewType>();
                        // Update collections

                        var noteVm = _noteVmFactory.Create(note, this);
                        _allNotes.Add(note);
                        //Notes.Insert(0, noteVm);

                        // Notification Logic (UI logic like Toasts often use a Service in MVVM)
                        //var toast = new SuccessToast("Note added successfully.");
                        //ToastContainer.Children.Add(toast);
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            Debug.WriteLine($"Insert VM: {this.GetHashCode()}");
                            Notes.Insert(0, noteVm); // UI-bound collection
                                                     //Notes.Add(noteVm); // UI-bound collection
                            Debug.WriteLine(Notes.Count);
                            CollectionViewSource.GetDefaultView(Notes).Refresh();

                        });

                       
                        WeakReferenceMessenger.Default.Send(new SuccessMessage("Note added successfully!"));
                        //await Application.Current.Dispatcher.InvokeAsync(() => { }, System.Windows.Threading.DispatcherPriority.Background);
                        await Task.Delay(2000);// Wait a little so animation shows

                        Log.Information("User {Username} added note '{Title}' successfully.", AppSession.Username, title);
                    }
                    else
                    {
                        MessageBox.Show(result.Message);
                    }
                }
                else // UPDATE MODE
                {
                    _editing.Title = title;
                    _editing.Content = content;
                    var result = await _notesService.UpdateNoteAsync(_editing.Model);

                    if (result.IsSuccess)
                    {
                        Log.Information("User {Username} updated note '{Title}'", AppSession.Username, title);
                        _editing = null;
                        AddButtonText = "Add Note";
                    }
                }

                // // ✅ Clear ONLY UI inputsClear inputs via properties
                NoteTitle = string.Empty;
                NoteContent = string.Empty;
                NoteCategory = string.Empty;

                // Close dialog
                IsAddNoteDialogOpen = false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in AddNoteCommand for user {Username}", AppSession.Username);
                MessageBox.Show($"Unexpected error: {ex.Message}");

                // Close dialog
                IsAddNoteDialogOpen = false;
            }
        }
        private void OpenAddNotePopup()
        {
            _editing = null;
            AddButtonText = "Add Note";
            IsAddNoteDialogOpen = true;


        }
        private void CloseAddNoteDialog()
        {
            IsAddNoteDialogOpen = false;
        }

        //public bool HasNotes => Notes != null && Notes.Count > 0;
        [ObservableProperty]
        public bool _hasNotes = true;
        public async Task LoadNotesAsync()
        {
            try
            {
                IsLoadingNotes = true; // 1. Start Progress Ring
                //ShowNotes = false;
                Log.Information("Loading notes for user {Username}...", AppSession.Username);

                // 2. Prepare the tasks (The Delay + The API Call)
                _notesService.SetJwtToken(AppSession.JwtToken);
                var loadingDelay = Task.Delay(700); // ⏳ Your minimum delay

                //Calling the API Services to get notes (this is the actual loading task)
                var loadTask = _notesService.GetNotesAsync();

                // 3. Wait for both to finish simultaneously
                await Task.WhenAll(loadingDelay, loadTask);

                var notes = await loadTask; // Get results from the task

                // STOP loading FIRST
                IsLoadingNotes = false;

                // 4. Handle Empty Results
                if (notes == null || notes.Count == 0)
                {
                    Notes.Clear();
                    _allNotes.Clear();
                    HasNotes = false;
                    //MessageBox.Show("No notes found for your role.");
                    return;
                }
                HasNotes = true;
                // 5. Clear lists before filling
                Notes.Clear();
                _allNotes.Clear();

                // 6. Role-Based Filtering
                IEnumerable<Note> notesToDisplay;
                if (AppSession.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    notesToDisplay = notes;
                    Log.Information("Admin loaded {Count} notes", notes.Count);
                }
                else
                {
                    notesToDisplay = notes.Where(n =>
                        n.CreatedBy.Equals(AppSession.Username, StringComparison.OrdinalIgnoreCase));
                    Log.Information("User loaded {Count} personal notes", notesToDisplay.Count());
                }

                // 7. Populate ViewModels
                foreach (var note in notesToDisplay)
                {
                    var noteVM = new NoteViewModel(note, _notesService, this, _colorState);
                    Notes.Add(noteVM);
                    _allNotes.Add(note);
                }

                ApplySearchTextFilter();
                Show_allNotes();
                ApplyCurrentFilter();

                //ShowNotes = true;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load notes");
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                IsLoadingNotes = false; // 9. Stop Progress Ring
                                        //ShowNotes = true;
                OnPropertyChanged(nameof(HasNotes));

            }
        }
        //public void RemoveNote(NoteViewType noteVm)
        //{


        //    // ✅ remove model
        //    _allNotes.Remove(noteVm);

        //    // ✅ remove VM (UI refresh)
        //    Notes.Remove(noteVm);


        //}
        public List<NoteViewModel> SelectedNotes =>
    Notes.Where(n => n.IsSelected).ToList();
        [RelayCommand]
        public async Task DeleteSelectedAsync()
        {
            //if (noteVm == null) return;

            var notesToDelete = SelectedNotes;
            foreach (var note in notesToDelete)
            {
                await _notesService.DeleteNoteAsync(note.Model.Id);
                _allNotes.Remove(note.Model);
                Notes.Remove(note);

            }
            ApplyCurrentFilter();
            // ✅ Raise event here (OWNER only)
            NoteDeletedSuccessfully?.Invoke("Note deleted successfully.");
        }
    }

}
