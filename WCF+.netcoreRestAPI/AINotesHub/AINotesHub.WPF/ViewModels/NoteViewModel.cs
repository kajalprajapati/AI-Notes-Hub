using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using AINotesHub.WPF.Enums;
using AINotesHub.WPF.Helpers;
using AINotesHub.WPF.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Client;
using Serilog;
using Windows.Services.Maps;
using Note = AINotesHub.Shared.Entities.Note;

namespace AINotesHub.WPF.ViewModels
{
    public partial class NoteViewModel : BaseViewModel
    {
        private readonly NotesApiService _notesApiService;
        private readonly MainViewModel _mainVm;
        public event Action<NoteViewModel> Deleted;
        public Note Model { get; set; }
        public ObservableCollection<Note> AllNotes { get; } = new();
        public ObservableCollection<Note> Notes { get; } = new();
        public event Action<NoteViewModel>? OpenRequested;


        [RelayCommand]
        private void SetFilter(NoteFilterType type)
        {
            SelectedFilter = type;
        }

        // --- GROUP 1: AUTOMATIC FIELDS ---
        #region Simple Backing Fields
        // and handles the "if != value" check and OnPropertyChanged automatically.
        [ObservableProperty] private string _username;
        [ObservableProperty] private string _email;
        [ObservableProperty] private string _imageName;
        [ObservableProperty] private string _imagePath;
        [ObservableProperty] private string _role;
        [ObservableProperty] private bool _isEditing;
        [ObservableProperty] private string _editableTitle;
        [ObservableProperty] private string _editableContent;
        [ObservableProperty] private bool _isSelected;
        [ObservableProperty] private NoteViewType _currentMode = NoteViewType.Active;
        [ObservableProperty] private NoteFilterType selectedFilter = NoteFilterType.All;
        [ObservableProperty]
        private ObservableCollection<string> suggestedTitles = new();

        [ObservableProperty]
        private string noteContent;

        [ObservableProperty]
        private string noteSummary;

        [ObservableProperty]
        private string noteTags;

        [ObservableProperty]
        private string noteTitle;

        [ObservableProperty]
        private bool isGenerating;



        private bool _isSummarizing;
        public bool IsSummarizing
        {
            get => _isSummarizing;
            set => SetProperty(ref _isSummarizing, value);
        }

        private bool _isAddNotePopupOpen;
        public bool IsAddNotePopupOpen
        {
            get => _isAddNotePopupOpen;
            set
            {
                _isAddNotePopupOpen = value;
                OnPropertyChanged();
            }
        }
        private bool _reminderControlsEnabled;
        public bool ReminderControlsEnabled
        {
            get => _reminderControlsEnabled;
            set => SetProperty(ref _reminderControlsEnabled, value);
        }
        private bool _isReminderOn;
        public bool IsReminderOn
        {
            get => _isReminderOn;
            set
            {
                SetProperty(ref _isReminderOn, value);

                // Enable/disable reminder inputs
                ReminderControlsEnabled = value;
            }
            //get => _isReminderOn;
            //set => SetProperty(ref _isReminderOn, value);
        }

        private DateTime? _reminderDate;
        public DateTime? ReminderDate
        {
            get => _reminderDate;
            set => SetProperty(ref _reminderDate, value);
        }
        private string _reminderTime = "09:00";
        public string ReminderTime
        {
            get => _reminderTime;
            set => SetProperty(ref _reminderTime, value);
        }
        #endregion

        // --- GROUP 2: MODEL FORWARDING ---
        #region MODEL FORWARDING Fields
        //Note:-You cannot use[ObservableProperty] here because the data is stored in Model.IsDeleted, not a local private field.
        public string Title
        {
            get => Model.Title;
            set => SetProperty(Model.Title, value, Model, (m, v) => m.Title = v);

        }
        public string Content
        {
            get => Model.Content;
            set => SetProperty(Model.Content, value, Model, (m, v) => m.Content = v);
        }
        public string Category
        {
            get => Model.Category;
            set => SetProperty(Model.Category, value, Model, (m, v) => m.Category = v);
        }
        public bool IsArchived
        {
            get => Model.IsArchived;
            set => SetProperty(Model.IsArchived, value, Model, (m, v) => m.IsArchived = v);
        }
        //public Brush CardBackground
        //{
        //    get => Model.CardBackground;
        //    set { if (Model.CardBackground != value) { Model.CardBackground = value; OnPropertyChanged(nameof(CardBackground)); } }
        //}
        private Brush _cardBackground = Brushes.White;
        public Brush CardBackground
        {
            get => _cardBackground;
            set
            {
                _cardBackground = value;
                OnPropertyChanged();
            }
        }
        public DateTime CreatedAt
        {
            get => Model.CreatedAt;
            set => SetProperty(Model.CreatedAt, value, Model, (m, v) => m.CreatedAt = v);
        }
        public DateTime UpdatedAt
        {
            get => (DateTime)Model.UpdatedAt;
            set => SetProperty((DateTime)Model.UpdatedAt, value, Model, (m, v) => m.UpdatedAt = v);
        }
        public string CreatedBy
        {
            get => Model.CreatedBy;
            set => SetProperty(Model.CreatedBy, value, Model, (m, v) => m.CreatedBy = v);
        }
        public bool IsDeleted
        {
            get => Model.IsDeleted;
            // SetProperty handles the 'if' check, the assignment, and the UI notification
            set => SetProperty(Model.IsDeleted, value, Model, (m, v) => m.IsDeleted = v);
        }
        public bool IsStarred
        {
            get => Model.IsStarred;
            set => SetProperty(Model.IsStarred, value, Model, (m, v) => m.IsStarred = v);
        }
        public bool IsImportant
        {
            get => Model.IsImportant;
            set => SetProperty(Model.IsImportant, value, Model, (m, v) => m.IsImportant = v);
        }
        #endregion

        partial void OnSelectedFilterChanged(NoteFilterType value)
        {
            //ApplyDateFilter(value);
        }

        #region Commands
        public ICommand OpenAddNotePopupCommand { get; }
        public ICommand CloseAddNotePopupCommand { get; }
        public ICommand OpenNoteCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand DeleteCommand { get; }
        //[RelayCommand]
        //private async Task GenerateTitle()

        #endregion

        [RelayCommand]
        private void Filter(string filter)
        {
            if (Enum.TryParse(filter, out NoteFilterType parsed))
            {
                SelectedFilter = parsed;
            }
        }

        private NoteViewModel? _editing;
        //private async Task OpenAddNotePopup()
        //{
        //    //_editing = null;           // ADD MODE
        //    ////AddButtonText = "Add Note";
        //    ////NoteTitle = string.Empty;
        //    ////NoteContent = string.Empty;
        //    ////NoteCategory = string.Empty;

        //    //IsAddNotePopupOpen = true; // 🔥 THIS OPENS POPUP
        //    if (_editing == null)
        //    {
        //        // ADD
        //    }
        //    else
        //    {
        //        // UPDATE
        //    }

        //    //IsAddNotePopupOpen = true;
        //    await DialogHost.Show(this, "AddNoteDialog");
        //}
        private void CloseAddNotePopup()
        {
            //IsAddNotePopupOpen = false;
            DialogHost.Close("AddNoteDialog");
        }
        public NoteViewModel(Note note)
        {
            Model = note;
        }
        public NoteViewModel(Note note,
NotesApiService notesService,
MainViewModel mainVm,
ColorStateViewModel colorState)
        {
            Model = note ?? new Note(); // 👈 MUST
            _notesApiService = notesService;
            _mainVm = mainVm;

            EditCommand = new RelayCommand(StartEdit); ;
            SaveCommand = new AsyncRelayCommand(UpdateNoteAsync);// includes reminder
            DeleteCommand = new AsyncRelayCommand(DeleteNoteAsync);
            CancelCommand = new RelayCommand(CancelEdit);
            OpenNoteCommand = new RelayCommand(OpenNote);
            CloseAddNotePopupCommand = new RelayCommand(CloseAddNotePopup);
        }
        private void OpenNote()
        {
            // Open edit popup
            // Show dialog
            // Just notify the View
            OpenRequested?.Invoke(this);
        }
        private void StartEdit()
        {

            EditableTitle = Model.Title;
            EditableContent = Model.Content;
            IsEditing = true;
        }
        private void CancelEdit()
        {
            IsEditing = false;
        }
        private async Task UpdateNoteAsync()
        {
            // 1️ Save basic fields
            Model.Title = EditableTitle;
            Model.Content = EditableContent;


            // 2️ 🔔 Handle Reminder HERE
            if (IsReminderOn && ReminderDate.HasValue)
            {
                if (TimeSpan.TryParse(ReminderTime, out var time))
                {
                    Model.IsReminderOn = true;
                    Model.ReminderDateTime =
                        ReminderDate.Value.Date + time;
                }
                else
                {
                    MessageBox.Show("Invalid time format. Use HH:mm");
                    return;
                }
            }
            else
            {
                Model.IsReminderOn = false;
                Model.ReminderDateTime = null;
            }


            //Model.ReminderDateTime = IsReminderOn ? (DateTime?)DateTime.Parse($"{ReminderDate?.ToShortDateString()} {ReminderTime}") : null;


            // 3️⃣ Call Service
            await _notesApiService.UpdateNoteAsync(Model);

            // 🔥 Notify UI manually (required)
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Content));

            IsEditing = false;
        }
        private async Task DeleteNoteAsync()
        {
            var result = await _notesApiService.DeleteNoteAsync(Model.Id);

            if (result.IsSuccess)
            {
                //_mainVm.RemoveNote(this);
                await _mainVm.DeleteSelectedAsync();

                Log.Warning(
                    "User {Username} deleted note '{Title}' (ID: {NoteId})",
                    AppSession.Username,
                    Title,
                    Model.Id
                );

                DialogHost.CloseDialogCommand.Execute(null, null);
            }
        }
        public void NotifyDeleted()
        {
            Deleted?.Invoke(this);
        }
        // Forward property changes for properties you expose
        // Expose properties for binding and notify WPF UI
        // Add more if you expose more properties (Title, Content, etc.)

        [RelayCommand]
        private async Task ToggleStarAsync()
        {
            IsStarred = !IsStarred;
            await _notesApiService.UpdateNoteAsync(Model);
        }
        [RelayCommand]
        private async Task ToggleImportantAsync()
        {
            IsImportant = !IsImportant;
            await _notesApiService.UpdateNoteAsync(Model);
        }

        [RelayCommand]
        private async Task GenerateTitle()
        {
            if (string.IsNullOrWhiteSpace(NoteContent))
                return;

            IsGenerating = true;

            try
            {
                var prompt = $@"
You are an intelligent assistant.

Analyze the following note and return:

Title: (max 8 words)
Summary: (2-3 lines)
Tags: (3-5 comma separated)

Note:
{NoteContent}
";
                var result = await AIService.Instance.GetAIResponse(prompt);
                string[] lines = result.Split('\n');

                NoteTitle = Extract(lines, "Title:");
                NoteSummary = Extract(lines, "Summary:");
                NoteTags = Extract(lines, "Tags:");
            }
            finally
            {
                IsGenerating = false;
            }
        }

        private string Extract(string[] lines, string key)
        {
            return lines
                .FirstOrDefault(x => x.StartsWith(key, StringComparison.OrdinalIgnoreCase))?
                .Split(':', 2)
                .LastOrDefault()
                ?.Trim();
        }

        private bool CanGenerateTitle()
        {
            return !string.IsNullOrWhiteSpace(NoteContent);

        }

        partial void OnNoteContentChanged(string value)
        {
            // Disambiguate by casting to IAsyncRelayCommand
            GenerateTitleCommand.NotifyCanExecuteChanged();
        }
    }

}





















