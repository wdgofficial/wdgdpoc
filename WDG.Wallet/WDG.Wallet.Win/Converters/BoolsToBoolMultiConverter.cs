// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace WDG.Wallet.Win.Converters
{
    public class BoolsToBoolMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(x => !(x is bool)))
                return null;

            var vs = values.Cast<bool>();
            var result = !vs.Any(x => !x);
            return result; ;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
