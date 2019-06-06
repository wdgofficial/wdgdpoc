// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Globalization;
using System.Windows.Data;

namespace WDG.Wallet.Win.Converters
{
    public class DateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is DateTime))
                return null;
            var date = (DateTime)value;
            if (parameter == null)
                return date.ToString("yyyy-MM-dd hh-mm-ss");
            else
                return date.ToString(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is string))
                return value;

            DateTime date = DateTime.UtcNow;
            DateTime.TryParse(value.ToString(), out date);

            return date;
        }
    }
}
