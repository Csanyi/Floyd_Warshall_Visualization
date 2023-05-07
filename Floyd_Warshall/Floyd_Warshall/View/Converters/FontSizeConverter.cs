using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Floyd_Warshall.View.Converters
{
    public class FontSizeConverter : IValueConverter
    {
        /// <returns>Smaller int, if the count of digits in value is more</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int n = System.Convert.ToInt32(value);

                if (n == int.MaxValue || n == 0)
                {
                    return 12;
                }

                int length = (int)Math.Floor(Math.Log10(Math.Abs(n)) + 1);

                if (length < 3)
                {
                    return 12;
                }
                else if (length < 5)
                {
                    return 9;
                }
                else if (length < 6)
                {
                    return 7;
                }
                else
                {
                    return 5;
                }
            }
            catch
            {
                return Binding.DoNothing;
            }
       
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
