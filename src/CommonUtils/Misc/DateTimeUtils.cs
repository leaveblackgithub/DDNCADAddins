using System;
using System.Threading;

namespace CommonUtils.Misc
{
    public static class DateTimeUtils
    {
        private static long _laststamp;

        public static long DateTimeToTimeStamp(DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalSeconds;
        }

        /// <summary>
        ///     DateTime转换为13位时间戳（单位：毫秒）
        /// </summary>
        /// <param name="dateTime"> DateTime</param>
        /// <returns>13位时间戳（单位：毫秒）</returns>
        public static long DateTimeToLongTimeStamp(DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;
        }

        private static long GetValidTimeStamp()
        {
            var stamp = DateTimeToLongTimeStampOfNow();
            if (stamp == _laststamp)
            {
                Thread.Sleep(1);
                stamp = DateTimeToLongTimeStampOfNow();
            }

            _laststamp = stamp;
            return stamp;
        }

        public static string AddTimeStampSuffix(string content)
        {
            return $"{content}{GetValidTimeStamp()}";
        }

        public static string AddTimeStampPrefix(string content)
        {
            return $"{GetValidTimeStamp()}{content}";
        }

        public static long DateTimeToLongTimeStampOfNow()
        {
            return DateTimeToLongTimeStamp(DateTime.Now);
        }
    }
}