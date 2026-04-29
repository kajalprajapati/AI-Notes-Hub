using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AINotesHub.WPF.ViewModels
{
    public class BaseViewModel : ObservableObject
    {

        private bool isBusy;

        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        private string title;
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        // Add common methods here
        //protected void Log(string msg)
        //{
        //    Debug.WriteLine(msg);
        //}
       
    }
}
