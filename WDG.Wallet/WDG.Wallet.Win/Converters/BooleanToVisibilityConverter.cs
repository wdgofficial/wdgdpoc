// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WDG.Wallet.Win.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!CheckNullAndType<bool>(value))
                return null;
            var b = (bool)value;

            if (CheckNullAndType<string>(parameter) && parameter.ToString().ToLower().Equals("reverse"))
            {
                b = !b;
            }

            return b ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!CheckNullAndType<Visibility>(value))
                return null;
            var visibility = (Visibility)value;
            var flag = false;
            if (CheckNullAndType<string>(parameter) && parameter.ToString().ToLower().Equals("reverse"))
            {
                flag = true;
            }
            if (flag)
            {
                return Visibility.Visible == visibility ? false : true;
            }
            else
            {
                return Visibility.Visible == visibility ? true : false;
            }
        }


        bool CheckNullAndType<T>(object value)
        {
            if (value == null || !(value is T))
                return false;

            return true;
        }
    }
}
