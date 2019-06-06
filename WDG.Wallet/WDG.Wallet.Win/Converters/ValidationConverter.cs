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
    public class ValidationConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            List<DependencyObject> result = new List<DependencyObject>();
            if (values == null)
                return result;
            
            foreach (var item in values)
            {
                if (item is DependencyObject)
                {
                    var obj = item as DependencyObject;
                    result.Add(obj);
                }
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
