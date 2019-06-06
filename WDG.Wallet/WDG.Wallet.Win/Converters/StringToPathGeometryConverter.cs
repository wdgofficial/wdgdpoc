// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WDG.Wallet.Win.Converters
{
    public class StringToPathGeometryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is string))
                return null;

            var key = value.ToString();
            var resource = Application.Current.FindResource(key);
            if (resource != null && resource is System.Windows.Media.Geometry)
            {
                return (System.Windows.Media.Geometry)resource;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
