using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PIM_Mini_Tests_WPF
{
    public class StatusToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Status val = (Status)value;
            switch (val)
            {
                case Status.Passed:
                    return "Passed";
                case Status.Failed:
                    return "Failed";
                case Status.NotRun:
                    return "Not run";
                case Status.Mixed:
                    return "Mixed";
                default:
                    return val.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = (string)value;
            switch (val)
            {
                case "Passed":
                    return Status.Passed;
                case "Failed":
                    return Status.Failed;
                case "Mixed":
                    return Status.Mixed;
                case "Not run":
                default:
                    return Status.NotRun;
            }
        }
    }
}
