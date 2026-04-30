using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;


namespace AINotesHub.WPF.Services
{
    public partial class NoteColorService : ObservableObject, INoteColorService
    {
        public string CurrentNoteColorHex { get; set; } = "#FFFFFFFF";

        //[ObservableProperty]

        //private Brush _currentNoteColor = Brushes.White;

        public void SaveSelectedColor(Brush color)
        {
            // Save to DB / file / settings / API
        }



    }
}
