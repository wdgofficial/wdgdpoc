// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace WDG.Wallet.Win.Converters
{
    public class ListCountToEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int len = 0;
            if (value == null || !(value is ICollection) || parameter == null || !int.TryParse(parameter.ToString(), out len))
                return true;

            var count = (value as ICollection).Count;

            if (count >= len)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    public class ListCountLessThanToEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int len = 0;
            if (value == null || !(value is ICollection) || parameter == null || !int.TryParse(parameter.ToString(), out len))
                return false;

            var count = (value as ICollection).Count;

            if (count <= len)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class ListCountMoreThanToEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int len = 0;
            if (value == null || !(value is ICollection) || parameter == null || !int.TryParse(parameter.ToString(), out len))
                return true;

            var count = (value as ICollection).Count;

            if (count >= len)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
