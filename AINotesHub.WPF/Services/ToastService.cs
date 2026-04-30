using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AINotesHub.WPF.Services
{
    public class ToastService
    {
        public static event Action<string>? OnShowSuccess;

        public static void ShowSuccess(string message)
        {
            OnShowSuccess?.Invoke(message);
        }
    }
}
