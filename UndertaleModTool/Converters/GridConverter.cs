using System;
using System.Linq;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UndertaleModTool
{
    public class GridConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double w = 0;
            double h = 0;
            if (values.All(x => x is double))
            {
                w = (double)values[0];
                h = (double)values[1];
            }
            else if (values.All(x => x is uint))
            {
                w = (uint)values[0];
                h = (uint)values[1];
            }
            else
                return new Rect();

            return new Rect(0, 0, w, h);
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
