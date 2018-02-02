using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PIM_Mini_Tests_WPF.RTC
{
    /// <summary>
    /// Ensures that the given value is within an accceptable range
    /// </summary>
    public class SleepConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val;
            try
            {
                val = System.Convert.ToInt32(value);
                if (val < 0)
                {
                    return 0;
                }
                return val;
            }
            catch (System.OverflowException)
            {
                return 2147483;
            }
        }
    }
}
