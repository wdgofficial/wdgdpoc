// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;

namespace WDG.Wallet.Win.Common.Utils
{
    public class DateTimeUtil
    {
        internal static long GetDateTimeStamp(DateTime dateTime)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            var timeStampDouble = (dateTime - startTime).TotalMilliseconds;
            return Convert.ToInt64(timeStampDouble);
        }

        internal static DateTime GetDateTime(long dateTimeStamp)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            var date = startTime.AddMilliseconds(dateTimeStamp);
            return date;
        }

    }
}
