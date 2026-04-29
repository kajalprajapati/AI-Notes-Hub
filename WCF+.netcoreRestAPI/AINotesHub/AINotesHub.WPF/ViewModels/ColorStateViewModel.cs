using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AINotesHub.WPF.ViewModels
{
    public class ColorStateViewModel : ObservableObject
    {
        private Brush _currentNoteColor = Brushes.White;

        public Brush CurrentNoteColor
        {
            get => _currentNoteColor;
            set => SetProperty(ref _currentNoteColor, value);
        }


        //public Brush CurrentNoteColor
        //{
        //    get => _currentNoteColor;
        //    set
        //    {
        //        if (_currentNoteColor == value) return;
        //        _currentNoteColor = value;
        //        OnPropertyChanged();
        //    }
        //}
    }
}
