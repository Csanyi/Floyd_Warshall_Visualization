using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Floyd_Warshall.View.Converters
{
	public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a bool value to Visibility type
        /// </summary>
        /// <returns>Visibility.Visible if value is true, otherwise Visibility.Collapsed</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                bool isVisible = System.Convert.ToBoolean(value);

                return isVisible ? Visibility.Visible : Visibility.Collapsed;
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
