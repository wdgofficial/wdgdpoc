// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WDG.Wallet.Win.Converters
{
    public class ConfirmationToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            var payment = (Payment)value;
            if (payment == null)
                return null;

            if (payment.Confirmations < 6 || (payment.Confirmations < 100 && payment.Category == "generate"))
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B8EFF"));

            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
