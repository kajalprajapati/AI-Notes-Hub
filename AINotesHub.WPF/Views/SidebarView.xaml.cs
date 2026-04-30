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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AINotesHub.WPF.UserControls;
using AINotesHub.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AINotesHub.WPF.Views
{
    /// <summary>
    /// Interaction logic for SidebarView.xaml
    /// </summary>
    public partial class SidebarView : UserControl
    {
        private MainViewModel _mainVm;

        public SidebarView()
        {
            //InitializeComponent();
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw; // keep this to not hide the real issue
            }
        }
        public SidebarView(MainViewModel mainVm, UserViewModel currentUser)
        {
            InitializeComponent();
            // Add user header to main UI
            _mainVm = mainVm;
            //DataContext = new SidebarViewModel(mainVm);
            DataContext = App._serviceProvider.GetRequiredService<SidebarViewModel>();
            //var vm = new SidebarViewModel(mainVm);
            //DataContext = vm;
            //ProfileImageContent.Content = new ProfileImageControl(currentUser);
        }
        private void BtnCalendar_Click(object sender, RoutedEventArgs e)
        {
            //CalendarClicked?.Invoke();
            // Load CalendarView into CalendarHost ContentControl
            CalendarHost.Content = new CalendarControl();
        }
    }
}
