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
using MaterialDesignThemes.Wpf;

namespace AINotesHub.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for SessionExpiredControl.xaml
    /// </summary>
    public partial class SessionExpiredControl : UserControl
    {
        public SessionExpiredControl()
        {
            InitializeComponent();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogHost.Close("RootDialogHost");
        }
    }
}
