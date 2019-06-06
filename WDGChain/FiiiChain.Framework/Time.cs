// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;

namespace FiiiChain.Framework
{
    public static class Time
    {
        public static DateTime EpochStartTime => new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

        public static long EpochTime => (long)(DateTime.UtcNow - EpochStartTime).TotalMilliseconds;

        public static long UnixTime => DateTimeOffset.Now.ToUnixTimeMilliseconds();

        public static long GetEpochTime(int year, int month, int day, int hour, int minute, int second)
        {
            return (long)(new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc) - EpochStartTime).TotalMilliseconds;
        }

        public static DateTime GetLocalDateTime(long timeStamp)
        {
            DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);//等价的建议写法
            TimeSpan toNow = new TimeSpan(timeStamp * 10000);
            return startTime.Add(toNow);
        }
    }
}
