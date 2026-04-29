using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AINotesHub.WPF.Factories;
using AINotesHub.WPF.Helpers;
using AINotesHub.WPF.Services;
using AINotesHub.WPF.ViewModels;
using MaterialDesignThemes.Wpf;
using Serilog;

namespace AINotesHub.WPF.UserControls.Dialogs
{
    /// <summary>
    /// Interaction logic for NoteDetailsDialog.xaml
    /// </summary>
    public partial class NoteDetailsDialog : UserControl 
    {
        //private readonly NoteViewModel _noteVm;
        //private readonly MainViewModel _mainVm;
        //private readonly NotesApiService _notesService;
        //private readonly ColorStateViewModel _colorState;// Empty variable (until you assign it).
        private readonly NoteDetailsDialogViewModelFactory _factory;
        public NoteDetailsDialog(
NotesApiService notesService,
NoteViewModel noteVm,
MainViewModel mainVm,
ColorStateViewModel colorState)
        //    public NoteDetailsDialog(NoteDetailsDialogViewModelFactory factory, NoteViewModel noteVm,
        //MainViewModel mainVm)
        {
            InitializeComponent();
            //_notesService = notesService;
            //_noteVm = noteVm;
            //_mainVm = mainVm;
            //DataContext = noteVm;  // same object

            DataContext = new NoteDetailsDialogViewModel(
             notesService,
             noteVm,
             mainVm,
             colorState);

            //_factory = factory;

            //DataContext = _factory.Create(noteVm, mainVm);

        }

        
    }
}
