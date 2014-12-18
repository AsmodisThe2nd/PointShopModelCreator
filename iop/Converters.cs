using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PSMC
{
    public class EnumToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value,
            Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (parameter.Equals(value.ToString()))
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if((bool)value)
            return  Enum.Parse(targetType, parameter.ToString());
            else
                return DependencyProperty.UnsetValue;

        }
        #endregion

    }
}
