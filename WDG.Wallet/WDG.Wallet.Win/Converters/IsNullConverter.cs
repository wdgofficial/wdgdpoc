// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Globalization;
using System.Windows.Data;

namespace WDG.Wallet.Win.Converters
{
    public class IsNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null || string.IsNullOrEmpty(parameter.ToString()))
            {
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                    return true;
            }
            else if (parameter.ToString() == "reverse")
            {
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                    return false;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
