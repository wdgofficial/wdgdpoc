// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WDG.Wallet.Win.Converters
{
    public class MultiLongConverter : IMultiValueConverter
    {
        double pow = Math.Pow(10, 8);

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return null;
            List<long?> result = new List<long?>();
            foreach (var item in values)
            {
                double value = 0d;
                if (double.TryParse(item.ToString(), out value))
                {
                    var longValue = System.Convert.ToInt64( value * pow);
                    result.Add(longValue);
                }
                else
                    result.Add(null);
            }
            return result ;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}