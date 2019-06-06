// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Wallet.Win.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace WDG.Wallet.Win.Converters
{
    public class TimestampToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeStamp= ConvertToDouble(value);
            if (timeStamp.IsFail)
                return null;

            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); 
            DateTime dt = startTime.AddMilliseconds(timeStamp.Value);
            
            if(parameter == null)
                return dt;

            var converter = parameter.ToString();
            return dt.ToString(converter);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateReuslt = ConvertToDate(value);
            if (dateReuslt.IsFail)
                return null;

            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); 
            var timeStampDouble = (dateReuslt.Value - startTime).TotalMilliseconds;
            var timeStamp = System.Convert.ToInt64(timeStampDouble);
            return timeStamp;
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


        public Result<DateTime> ConvertToDate(object value)
        {
            var result = new Result<DateTime>();
            if (value == null)
            {
                result.IsFail = true;
                return result;
            }

            var str = value.ToString();
            DateTime dateTime;
            if (DateTime.TryParse(str, out dateTime))
            {
                result.IsFail = false;
                result.Value = dateTime;
                return result;
            }

            result.IsFail = true;
            return result;
        }
    }
}
