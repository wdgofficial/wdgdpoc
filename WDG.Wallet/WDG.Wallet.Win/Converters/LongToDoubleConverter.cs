// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Globalization;
using System.Windows.Data;

namespace WDG.Wallet.Win.Converters
{
    public class LongToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is long))
                return null;

            var longValue = long.Parse(value.ToString());

            if (parameter != null && parameter.ToString().ToLower() == "true")
            {
                if (!string.IsNullOrEmpty(tempStr))
                    return tempStr;
                return GetDoubleStr(longValue);
            }

            var doubleValue = longValue / Math.Pow(10, 8);
            return doubleValue.ToString("0.00000000");
        }

        string GetDoubleStr(long l)
        {
            var tempL = l;
            int count = 0;
            bool flag = true;
            while (flag)
            {
                var d = l % 10;
                if (d > 0 || l == 0)
                {
                    flag = false;
                }
                else
                {
                    l = l / 10;
                    count++;
                }
            }
            if (count >= 8)
            {
                return (tempL / Math.Pow(10, 8)).ToString();
            }
            else
            {
                var format = "0.";
                for (int i = 0; i < 8 - count; i++)
                {
                    format += "0";
                }
                return (tempL / Math.Pow(10, 8)).ToString(format);
            }
        }

        string tempStr = "";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null)
                    return 0;

                var str = value.ToString();
                double doubleValue;
                if (double.TryParse(str, out doubleValue))
                {
                    if (parameter != null && parameter.ToString().ToLower() == "true")
                        tempStr = str;

                    var longVlaue = doubleValue * Math.Pow(10, 8);
                    return System.Convert.ToInt64(longVlaue);
                }
                return 0;
            }
            catch
            {
                return null;
            }
        }
    }
}
