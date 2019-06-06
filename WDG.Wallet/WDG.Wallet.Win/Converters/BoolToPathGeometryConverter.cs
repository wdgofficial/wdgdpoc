// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WDG.Wallet.Win.Converters
{
    public class BoolToPathGeometryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is bool) || parameter == null)
                return null;

            var s = parameter.ToString().Split('|');
            if (s.Length != 2)
                return null;

            var key = ((bool)value) ? s[0] : s[1];

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


    public class BoolToStringGeometryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is bool) || parameter == null)
                return null;

            var s = parameter.ToString().Split('|');
            if (s.Length != 2)
                return null;

            var key = ((bool)value) ? s[0] : s[1];
            
            return LanguageService.Default.GetLanguageValue(key);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
