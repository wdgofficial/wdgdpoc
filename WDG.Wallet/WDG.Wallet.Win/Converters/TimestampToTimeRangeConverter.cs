// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Models;
using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace WDG.Wallet.Win.Converters
{
    public class TimestampToTimeRangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeStamp = ConvertToDouble(value);
            if (timeStamp.IsFail)
                return null;
            
            var timeSpan = TimeSpan.FromMilliseconds(timeStamp.Value);
            
            if (parameter == null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                if (timeSpan.Days > 0)
                {
                    var days = LanguageService.Default.GetLanguageValue("days");
                    stringBuilder.AppendFormat("{0} {1}", timeSpan.Days, days);
                }
                if (timeSpan.Hours > 0)
                {
                    var hours = LanguageService.Default.GetLanguageValue("hours");
                    stringBuilder.AppendFormat("{0} {1}", timeSpan.Hours, hours);
                }
                if (timeSpan.Minutes > 0)
                {
                    var minutes = LanguageService.Default.GetLanguageValue("minutes");
                    stringBuilder.AppendFormat("{0} {1}", timeSpan.Minutes, minutes);
                }
                if (timeSpan.Seconds > 0)
                {
                    var seconds = LanguageService.Default.GetLanguageValue("seconds");
                    stringBuilder.AppendFormat("{0} {1}", timeSpan.Seconds, seconds);
                }
                return stringBuilder.ToString();
            }

            var converter = parameter.ToString();
            return timeSpan.ToString(converter);
        }

        public Result<double> ConvertToDouble(object value)
        {
            var result = new Result<double>();
            if (value == null)
            {
                result.IsFail = true;
                return result;
            }

            var str = value.ToString();
            var l = 0L;
            if (long.TryParse(str, out l))
            {
                result.IsFail = false;
                result.Value = l;
                return result;
            }

            result.IsFail = true;
            return result;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
