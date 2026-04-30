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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AINotesHub.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for SuccessToast.xaml
    /// </summary>
    public partial class SuccessToast : UserControl
    {
        public SuccessToast(string message)
        {
            InitializeComponent();
            MessageText.Text = message;

            FadeIn();
            AutoClose();
        }

        private void SuccessToast_Loaded(object sender, RoutedEventArgs e)
        {
            FadeIn();
            AutoClose();
        }

        // 🔵 Fade-in animation (0 → 1)
        private void FadeIn()
        {
            var anim = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            MainBorder.BeginAnimation(OpacityProperty, anim);
        }

        // 🔴 Wait, then fade out & remove control
        private async void AutoClose()
        {
            // 2.5 seconds visible
            await Task.Delay(2500);

            var anim = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            anim.Completed += (s, e) =>
            {
                // Remove toast after fade-out
                if (this.Parent is Panel panel)
                    panel.Children.Remove(this);
            };

            MainBorder.BeginAnimation(OpacityProperty, anim);
        }
    }
}
