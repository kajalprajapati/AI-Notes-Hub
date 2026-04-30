using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using AINotesHub.WPF.Enums;
using AINotesHub.WPF.Helpers;
using AINotesHub.WPF.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging.Messages;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Windows.System;
//using Windows.UI.Xaml.Media;
//using Brush = System.Windows.Media.Brush;
//using Brush = Windows.UI.Xaml.Media.Brush;
//using Brush = System.Windows.Media.Brush;
//using SolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace AINotesHub.WPF.ViewModels
{
    public partial class SidebarViewModel : ObservableObject
    {
        private readonly INoteColorService _noteColorService;
        private readonly MainViewModel _mainVM;

        [ObservableProperty] private NoteViewType _currentMode = NoteViewType.Active;
        [ObservableProperty] private Brush _currentNoteColorHex;
        [ObservableProperty] private bool isColorSelected;
        public MainViewModel MainVm { get; }
        public User CurrentUser { get; }
        [ObservableProperty] private ObservableCollection<Brush> _noteColors;
        [ObservableProperty] private ObservableCollection<NavigationItem> navigationItems;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsSearchVisible))]
        private NavigationItem selectedNavigationItem;

        public bool IsSearchVisible =>
    SelectedNavigationItem?.Mode == NoteViewType.SearchNotes;
        partial void OnSelectedNavigationItemChanged(NavigationItem value)
        {
            if (value != null)
            {
                OnNavigationChanged(value.Mode);
                OnPropertyChanged(nameof(IsSearchVisible));

                //if (SelectedNavigationItem?.Name == "SearchNotes")
                //{
                //    IsSearchVisible = true;
                //}
            }
        }

        //public Brush SelectedColor
        //{
        //    get => _selectedColor;
        //    set
        //    {
        //        // 1. THE GUARD: Only proceed if the color is actually NEW
        //        if (_selectedColor == value) return;

        //        _selectedColor = value;
        //        OnPropertyChanged(nameof(SelectedColor));
        //        //OnPropertyChanged();
        //        Debug.WriteLine($"Color changed to: {value}");
        //        // 2. Logic only runs once per change
        //        OnColorSelected(value);
        //    }
        //}
        public void OnSelectedColorChanged(Brush value)
        {
            Debug.WriteLine($"Color changed to: {value}");
            OnColorSelected(value);

            // 🔹 THIS updates your ToggleButton automatically
            IsColorSelected = value != null;
        }
        // 3. Property to track which item is clicked
        private void OnNavigationChanged(NoteViewType mode)
        {
            CurrentMode = mode;
            //ApplyCurrentFilter(); // This refreshes the right panel cards
        }
        // Add this field to store the command instance
        private RelayCommand<Brush?> _selectColorCommand;
        public RelayCommand<Brush?> SelectColorCommand => _selectColorCommand;

        private readonly ColorStateViewModel _colorState;
        public SidebarViewModel()
        {
            Init();
            _selectColorCommand = new RelayCommand<Brush?>(OnColorSelected);   // THIS populates the palette

        }

        public SidebarViewModel(INoteColorService colorService, ColorStateViewModel colorState) : this()

        {
            _noteColorService = colorService;
            // _mainVM = mainVm ?? throw new ArgumentNullException(nameof(mainVm));

            //    SelectedColor = NoteColors.FirstOrDefault();
            _colorState = colorState
            ?? throw new ArgumentNullException(nameof(colorState));

        }


        private void Init() // ✅ MUST be private
        {
            // Initialize the collection
            navigationItems = new ObservableCollection<NavigationItem>
        {
                            new NavigationItem { Name = "Dashboard",Icon = PackIconKind.ViewDashboard, Mode = NoteViewType.Dashboard },
            new NavigationItem { Name = "My Notes", Icon = PackIconKind.Inbox, Mode = NoteViewType.Active },
            new NavigationItem { Name = "Search Notes", Icon = PackIconKind.Magnify, Mode = NoteViewType.SearchNotes },
            new NavigationItem { Name = "Starred", Icon = PackIconKind.StarOutline, Mode = NoteViewType.Starred },
            new NavigationItem { Name = "Important", Icon = PackIconKind.StarOutline, Mode = NoteViewType.Important },
            new NavigationItem { Name = "Calendar", Icon = PackIconKind.Calendar, Mode = NoteViewType.Calendar },
            new NavigationItem { Name = "Archive", Icon = PackIconKind.Archive, Mode = NoteViewType.Archived },
            new NavigationItem { Name = "Trash", Icon = PackIconKind.Delete, Mode = NoteViewType.Trash }
        };

            // SAFETY: Only set if items exist
            selectedNavigationItem = navigationItems.Count > 0
                ? navigationItems[0]
                : null;

            //SelectedNavigationItem = NavigationItems.FirstOrDefault();

            _noteColors = new ObservableCollection<Brush>(NoteColorPalette.Default);
            CurrentNoteColorHex = _noteColors.FirstOrDefault();

            Debug.WriteLine(Object.ReferenceEquals(
    _colorState,
    App._serviceProvider.GetRequiredService<ColorStateViewModel>()
));

        }

        [ObservableProperty]
        private string _lastHex;

        private void OnColorSelected(Brush color)
        {
            System.Diagnostics.Debug.WriteLine("COLOR COMMAND FIRED");

            if (color == null) return;

            //// 1️⃣ Save to service
            //var hex = color.ToString();
            //_noteColorService.CurrentNoteColorHex = hex;

            // 2️⃣ Update UI
            //_mainVM.CurrentNoteColor = color;
            _colorState.CurrentNoteColor = color;
            CurrentNoteColorHex = color;
            Debug.WriteLine($"Sidebar color set: {color}");

        }
    }
}

