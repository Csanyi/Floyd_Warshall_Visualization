using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Floyd_Warshall.View.Converters
{
	public class IntWithInfConverter : IValueConverter
    {
        /// <returns>Unicode charecter infinity if value is int.MaxValue,
        /// otherwise the value's string representation</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int valueInt = System.Convert.ToInt32(value);

                if (valueInt == int.MaxValue)
                {
                    return "\u221E";
                }
                else
                {
                    return valueInt.ToString();
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
