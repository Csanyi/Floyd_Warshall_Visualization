using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Floyd_Warshall.View.Converters
{
    public class FontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int n = System.Convert.ToInt32(value);

            if(n != int.MaxValue && Math.Floor(Math.Log10(Math.Abs(n))) > 1)
            {
                return 9;
            }
            else
            {
                return 12;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
