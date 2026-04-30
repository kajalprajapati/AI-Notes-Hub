using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using AINotesHub.WPF.Helpers;
using AINotesHub.WPF.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Serilog;

namespace AINotesHub.WPF.ViewModels
{
    public partial class NoteDetailsDialogViewModel : ObservableObject
    {
        // Implement properties and commands for NoteDetailsDialog here
        private readonly NotesApiService _notesService;
        private readonly MainViewModel _mainVm;
        private readonly NoteViewModel _noteVm;
        private readonly ColorStateViewModel _colorState;
        private readonly AIService _aiService;
        private readonly AIUsageService _usageService = new();
        private readonly ReminderService _reminderService;

        // ---------------- NOTE ----------------

        public NoteViewModel Note { get; }

        private string _summary;
        public string Summary
        {
            get => _summary;
            set => SetProperty(ref _summary, value);
        }
        // ---------------- COMMAND ----------------
        public IAsyncRelayCommand DeleteNoteCommand { get; }
        public IRelayCommand SummarizeCommand { get; }
        public ICommand AiSummarizeCommand { get; }
        public ICommand AiImproveCommand { get; }
        public ICommand AiTitleCommand { get; }
        public ICommand AiTasksCommand { get; }
        public ICommand AiReminderCommand { get; }
        public NoteDetailsDialogViewModel(NotesApiService notesService,
        NoteViewModel noteVm,
        MainViewModel mainVm,
        ColorStateViewModel colorState)
        {
            //_colorState = colorState;
            _notesService = notesService;
            _noteVm = noteVm;
            _mainVm = mainVm;


            Note = noteVm;
            _colorState = colorState ?? throw new ArgumentNullException(nameof(colorState));
            _aiService = new AIService();
            // _colorState.PropertyChanged += ColorState_PropertyChanged;

            DeleteNoteCommand = new AsyncRelayCommand(DeleteNoteAsync);
            SummarizeCommand = new RelayCommand(async () => await SummarizeNote());//Work offline
            AiSummarizeCommand = new RelayCommand(async () => await AiSummarize());//Work online
            AiImproveCommand = new RelayCommand(async () => await AiImprove());
            AiTitleCommand = new RelayCommand(async () => await AiTitle());
            AiTasksCommand = new RelayCommand(async () => await AiTasks());
            AiReminderCommand = new RelayCommand(async () => await AiReminder());

        }
        public async Task SummarizeNote()
        {
            try
            {
                //string BeforeSummarized = Note.Content;
                string BeforeSummarized = "@ Today I went to office!! 🏢... Had meeting.\r\nWorked on AI notes app.\r\nNeed to finish reminder feature. ";

                //String AfterSummarized = 

                // 1.Initial Trim
                string clean = BeforeSummarized.Trim();

                // 2. The Character Filter (Replaces your old Line 1 and Line 3)
                // This keeps ONLY letters, numbers, spaces, and . , ! ? 
                // We replace everything else with a space " " to prevent words from sticking together.
                clean = Regex.Replace(clean, @"[^a-zA-Z0-9\s\.,!\?]", " ");

                // 3. The Whitespace Smoother (Replaces your old Line 2 and Line 4)
                // This turns tabs, newlines, and multiple spaces into a single space.
                clean = Regex.Replace(clean, @"\s+", " ").Trim();

                // 4. Data Extraction
                var sentences = clean.Split('.', StringSplitOptions.RemoveEmptyEntries);
                string[] words = clean.Split(" ");
                Note.EditableContent = clean;

            }
            catch (Exception ex)
            {

                //throw;
            }
        }
        private async Task AiSummarize()
        {
            //var prompt = $"Summarize this note:\n{Note.Content}";

            //var result = await _aiService.AskAIAsync(prompt);

            if (IsSummarizing) // 🚫 prevent double click
                return;

            if (!_usageService.CanUseAI())
            {
                Summary = "⚠️ Daily AI limit reached. Try tomorrow.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Note.Content))
                return;

            try
            {
                IsSummarizing = true; // 🔒 Lock button

                Summary = "⏳ Summarizing...";


                var result = await _aiService.SummarizeAsync(Note.Content);

                Summary = result;
                Note.EditableContent = result;
                // ✅ Count usage
                _usageService.Increase();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine(ex.Message);
                var msg = ex.Message.ToLower();

                if (msg.Contains("quota") || msg.Contains("billing"))
                {
                    Summary = "⚠️ AI limit reached. Please check your plan or try tomorrow.";
                }
                else if (msg.Contains("429") || msg.Contains("too many"))
                {
                    Summary = "⚠️ Too many requests. Please wait a moment.";
                }
                else
                {
                    Summary = "❌ Network error. Please check your internet.";
                }
            }
            catch (Exception ex)
            {
                Summary = "❌ Failed to summarize.";
            }
            finally
            {
                IsSummarizing = false; // 🔓 Unlock button
            }


        }
        private async Task AiImprove()
        {
            var prompt = $"Improve this English professionally:\n{Note.Content}";

            var result = await _aiService.AskAIAsync(prompt);

            Note.EditableContent = result;
        }
        private async Task AiTitle()
        {
            var prompt = $"Generate a short title for this note:\n{Note.Content}";

            var result = await _aiService.AskAIAsync(prompt);

            Note.EditableTitle = result;
        }
        private async Task AiTasks()
        {
            var prompt = $"Extract TODO list from this note:\n{Note.Content}";

            var result = await _aiService.AskAIAsync(prompt);

            Note.EditableContent = result;
        }
        private async Task AiReminder()
        {
            var prompt = $"Find any date or reminder in this note:\n{Note.Content}";

            var result = await _aiService.AskAIAsync(prompt);

            MessageBox.Show(result, "Reminder Found");
        }
        //[ObservableProperty]
        //[NotifyPropertyChangedFor(nameof(CanSummarize))]
        //private bool isSummarizing;

        [ObservableProperty]
        private bool isSummarizing;
        // UI-friendly property (No Converter 😎)
        public Visibility LoadingVisibility =>
            IsSummarizing ? Visibility.Visible : Visibility.Collapsed;

        // Auto-update when IsSummarizing changes
        partial void OnIsSummarizingChanged(bool value)
        {
            OnPropertyChanged(nameof(LoadingVisibility));
            SummarizeCommand.NotifyCanExecuteChanged();
        }
        //public bool CanSummarize => !IsSummarizing;

        //[RelayCommand]
        //[RelayCommand(CanExecute = nameof(CanSummarize))]

        //public Brush CurrentNoteColor => _colorState.CurrentNoteColor;
        //private void ColorState_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == nameof(ColorStateViewModel.CurrentNoteColor))
        //    {
        //        OnPropertyChanged(nameof(CurrentNoteColor));
        //        Debug.WriteLine("NoteVM notified");
        //    }
        //}

        //private async Task DeleteNoteAsync1()
        //{
        //    var result = await _notesService.DeleteNoteAsync(Note.Model.Id);

        //    if (result.IsSuccess)
        //    {
        //        _mainVm.RemoveNote(this);

        //        Log.Warning(
        //            "User {Username} deleted note '{Title}' (ID: {NoteId})",
        //            AppSession.Username,
        //            Note.Title,
        //            Note.Model.Id
        //        );

        //        DialogHost.CloseDialogCommand.Execute(null, null);
        //    }
        //}

        private async Task DeleteNoteAsync()
        {
            //if (sender is Button btn && btn.DataContext is NoteViewModel noteVm)
            //{
            // ✅ Call API

            //if (MessageBox.Show("Are you sure to delete this note?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //{
            if (MessageBox.Show(
    "Are you sure to delete this note?",
    "Confirm",
    MessageBoxButton.YesNo)
!= MessageBoxResult.Yes)
                return;
            try
            {
                var result = await _notesService.DeleteNoteAsync(Note.Model.Id);

                if (!result.IsSuccess)
                    return;

                if (result.IsSuccess)
                {
                    // 🔥 Notify MainViewModel instead of touching collections
                    Note.NotifyDeleted();
                    _mainVm.DeleteSelectedAsync();
                    // 🔔 Notify UI (event / messenger / callback)

                    _mainVm.NoteDeletedSuccessfully += message =>
                    {
                        ToastService.ShowSuccess("Note deleted successfully");
                        //ToastContainer.Children.Add(new SuccessToast(message));
                    };
                    //var toast = new SuccessToast("Note deleted successfully.");
                    //ToastContainer.Children.Add(toast);
                    await Application.Current.Dispatcher.InvokeAsync(() => { }, System.Windows.Threading.DispatcherPriority.Background);
                    await Task.Delay(5000);// Wait a little so animation shows
                    Log.Warning("User {Username} deleted note '{Title}' (ID: {NoteId})",
                        AppSession.Username, Note.Title, Note.Model.Id);

                    // ✅ CLOSE THE DIALOG
                    DialogHost.Close("RootDialogHost", "Deleted");
                    //await DialogHost.Show(detailView, "RootDialogHost");


                }
                else
                {
                    Log.Error("Failed to delete note '{Title}' for user {Username}: {Message}",
                        Note.Title, AppSession.Username, result.Message);
                    MessageBox.Show(result.Message);
                }


            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting note '{Title}' (ID: {NoteId}) by user {Username}",
                    Note.Title, Note.Model.Id, AppSession.Username);
            }
        }
    }
}

//}
//}
