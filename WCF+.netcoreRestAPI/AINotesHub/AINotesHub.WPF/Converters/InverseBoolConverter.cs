
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AINotesHub.WPF.Converters
{
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value is bool b)
            //    return b ? Visibility.Collapsed : Visibility.Visible;
            return value is bool b && !b
           ? Visibility.Visible
           : Visibility.Collapsed;

           // return Visibility.Visible;
        }

        //public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    //throw new NotImplementedException();
        //    return (Visibility)value == Visibility.Visible;
        //}
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
