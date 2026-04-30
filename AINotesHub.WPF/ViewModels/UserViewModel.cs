using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using AINotesHub.WPF.Helpers;
using AINotesHub.WPF.Properties;

namespace AINotesHub.WPF.ViewModels
{
    public partial class UserViewModel : ObservableObject
    {
        //private string _username;
        //private string _email;
        //private string _role;
        //private string _imagePath; // optional if using profile picture

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _role;

        [ObservableProperty]
        private string _imagePath;

        public UserViewModel()
        {
            // Load user data from settings or API
            _username = Settings.Default.Username;
            _email = Settings.Default.Email;
            _role = Settings.Default.Role;
            _imagePath = Settings.Default.ProfileImagePath;
        }



        //public string Username
        //{
        //    get => _username;
        //    set
        //    {
        //        if (_username != value)
        //        {
        //            _username = value;
        //            OnPropertyChanged(nameof(Username));
        //        }
        //    }
        //}


    }
}
