﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Floyd_Warshall.View.Converters
{
    public class IntWithInfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int valueInt = (int)value;

            if(valueInt == int.MaxValue)
            {
                return "\u221E";
            }
            else
            {
                return valueInt.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}