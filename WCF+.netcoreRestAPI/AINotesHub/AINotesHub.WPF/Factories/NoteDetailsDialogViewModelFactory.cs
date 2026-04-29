using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AINotesHub.WPF.Services;
using AINotesHub.WPF.ViewModels;

namespace AINotesHub.WPF.Factories
{
    public class NoteDetailsDialogViewModelFactory
    {
        private readonly NotesApiService _notesService;
        private readonly ColorStateViewModel _colorState;
        private readonly ReminderService _reminderService;

        public NoteDetailsDialogViewModelFactory(
        NotesApiService notesService,
        ColorStateViewModel colorState)
        {
            _notesService = notesService;
            _colorState = colorState;
            _reminderService =
        new ReminderService(new AIService());
        }

        public NoteDetailsDialogViewModel Create(
            NoteViewModel noteVm,
            MainViewModel mainVm)
        {
            return new NoteDetailsDialogViewModel(
                _notesService,
                noteVm,
                mainVm,
                _colorState);
        }
    }
}
