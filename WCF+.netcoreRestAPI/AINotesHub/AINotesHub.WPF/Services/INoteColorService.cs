using System.Windows.Media;

namespace AINotesHub.WPF.Services
{
    public interface INoteColorService
    {
        string CurrentNoteColorHex { get; set; }

        //Brush CurrentNoteColor { get; set; }

        //event EventHandler ColorChanged;

    }
}
