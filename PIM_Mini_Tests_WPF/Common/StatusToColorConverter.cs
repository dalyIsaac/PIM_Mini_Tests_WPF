using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PIM_Mini_Tests_WPF.Common
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Status val = (Status)value;
            switch (val)
            {
                case Status.Passed:
                    return "#00c660"; // Green
                case Status.Failed:
                    return "#ff0000"; // Red
                case Status.NotRun:
                    return "#008cff"; // Blue
                case Status.Mixed:
                    return "#ffaa00"; // Orange
                default:
                    return "#000000"; // Black
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
