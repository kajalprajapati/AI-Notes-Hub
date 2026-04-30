using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AINotesHub.WPF.ViewModels;

namespace AINotesHub.WPF
{
    /// <summary>
    /// Interaction logic for NoteEditorWindow.xaml
    /// </summary>
    public partial class NoteEditorWindow : Window
    {
        public NoteEditorWindow(NoteViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
