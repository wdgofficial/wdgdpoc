// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WDG.Wallet.Win.Converters
{
    public class CompareToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return true;

            List<string> @params = new List<string>();
            if (parameter.ToString().Contains("|"))
            {
                @params.AddRange(parameter.ToString().Split('|'));
            }
            else
            {
                @params.Add(parameter.ToString());
            }

            var isContains = @params.Contains(value.ToString());
            return isContains ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}