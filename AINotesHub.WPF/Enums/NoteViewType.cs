using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AINotesHub.WPF.Enums
{
    public enum NoteViewType
    {
        Dashboard,
        Active,
        ActiveNotes,
        Archived,
        Trash,     // Use 'Trash' to match your BtnTrash and ShowTrashCommand
        Calendar,
        Starred,
        Important,
        SearchNotes
    }

    
}
