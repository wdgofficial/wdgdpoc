// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WDG.Wallet.Win.Converters
{
    public class EnumToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null || !(value is Enum) || !(parameter is Enum))
                return Visibility.Visible;

            if (value.GetType() != parameter.GetType())
                return Visibility.Visible;

            var valueEnum = value as Enum;
            var parameterEnum = parameter as Enum;
            
            if (valueEnum.HasFlag(parameterEnum))
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
