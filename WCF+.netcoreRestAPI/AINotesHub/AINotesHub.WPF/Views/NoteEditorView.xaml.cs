using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AINotesHub.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;


namespace AINotesHub.WPF.Views
{
    /// <summary>
    /// Interaction logic for NoteEditorView.xaml
    /// </summary>
    public partial class NoteEditorView : UserControl
    {

        // For XAML Designer
        public NoteEditorView()
        {
            InitializeComponent();
            
        }

        // For DI
        public NoteEditorView(NoteViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            
        }
    }
}
