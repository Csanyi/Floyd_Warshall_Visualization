﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Floyd_Warshall.View.Converters
{
    public class IntBoolTupleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int i = System.Convert.ToInt32(values[0]);
            bool b = System.Convert.ToBoolean(values[1]);

            return new Tuple<int, bool>(i, b);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}