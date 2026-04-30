using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace AINotesHub.WPF.Converters
{
    public class StarColorConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isStarred && isStarred)
            {
                return new SolidColorBrush(Colors.Gold);
            }
            return new SolidColorBrush(Colors.Gray); // Or Transparent
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
