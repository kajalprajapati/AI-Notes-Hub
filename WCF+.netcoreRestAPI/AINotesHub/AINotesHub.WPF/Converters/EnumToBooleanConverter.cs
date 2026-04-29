using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace AINotesHub.WPF.Converters
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value.ToString().Equals(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return Enum.Parse(targetType, parameter.ToString());

            return Binding.DoNothing;
        }
    }
}
