using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;

namespace AINotesHub.WPF.Converters
{
    public class StarIconConverter :IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isStarred)
            {
                // Returns filled star if true, outline if false
                return isStarred ? PackIconKind.Star : PackIconKind.StarOutline;
            }
            return PackIconKind.StarOutline;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
