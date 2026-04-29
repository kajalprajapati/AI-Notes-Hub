using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AINotesHub.Shared.Entities;
using AINotesHub.WPF.Services;
using AINotesHub.WPF.ViewModels;

namespace AINotesHub.WPF.Factories
{
    public class NoteViewModelFactory
    {
        private readonly NotesApiService _notesService;
        private readonly MainViewModel _mainVm;
        private readonly ColorStateViewModel _colorState;

        public NoteViewModelFactory(
            NotesApiService notesService,
            ColorStateViewModel colorState)
        {
            //_notesService = notesService;
            //_mainVm = mainVm;
            //_colorState = colorState;
            _notesService = notesService ?? throw new ArgumentNullException(nameof(notesService));
            _colorState = colorState ?? throw new ArgumentNullException(nameof(colorState));
        }

        public NoteViewModel Create(Note model, MainViewModel mainVm)
        {
            return new NoteViewModel(model, _notesService, _mainVm, _colorState);
        }
    }
}
