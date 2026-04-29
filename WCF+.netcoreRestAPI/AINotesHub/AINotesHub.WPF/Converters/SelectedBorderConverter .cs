using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Windows.UI.Xaml;

namespace AINotesHub.WPF.Converters
{
    public class SelectedBorderConverter : IValueConverter
    {
        ////public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        ////    => (bool)value ? Brushes.MediumPurple : Brushes.Transparent;
        //// Border color when item is selected
        public Brush SelectedBrush { get; set; } = Brushes.MediumPurple;

        // Border color when item is NOT selected
        public Brush UnselectedBrush { get; set; } = Brushes.Transparent;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected)
            {
                return isSelected ? SelectedBrush : UnselectedBrush;
            }

            return UnselectedBrush;
            //return (value is bool isSelected && isSelected)
            //? Brushes.Black
            //: Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    if (value is Color color)
        //        return new SolidColorBrush(color);
        //    return DependencyProperty.UnsetValue;
        //}
        //public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    throw new NotImplementedException();
        //}

    }
}
