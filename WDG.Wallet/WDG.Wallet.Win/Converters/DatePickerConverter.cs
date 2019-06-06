// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Globalization;
using System.Windows.Data;

namespace WDG.Wallet.Win.Converters
{
    public sealed class DatePickerConverter
    {

        public static TrueToFalseConverter _trueToFalseConverter;
        public static TrueToFalseConverter TrueToFalseConverter
        {
            get
            {
                if (_trueToFalseConverter == null)
                    _trueToFalseConverter = new TrueToFalseConverter();
                return _trueToFalseConverter;
            }
        }
    }

    public sealed class TrueToFalseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (bool)value;
            return !v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
