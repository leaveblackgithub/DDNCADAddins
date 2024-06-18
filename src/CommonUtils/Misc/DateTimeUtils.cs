using System;
using System.Threading;

namespace CommonUtils.Misc
{
    public static class DateTimeUtils
    {
        private static long _lastLongStamp;
        private static long _lastShortStamp;

        public static long DateTimeToShortStamp(DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalSeconds;
        }

        /// <summary>
        ///     DateTime转换为13位时间戳（单位：毫秒）
        /// </summary>
        /// <param name="dateTime"> DateTime</param>
        /// <returns>13位时间戳（单位：毫秒）</returns>
        public static long DateTimeToLongStamp(DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;
        }

        private static long GetValidLongTimeStamp()
        {
            var stamp = DateTimeToLongStampOfNow();
            if (stamp == _lastLongStamp)
            {
                Thread.Sleep(1);
                stamp = DateTimeToLongStampOfNow();
            }

            _lastLongStamp = stamp;
            return stamp;
        }
        private static long GetValidShortTimeStamp()
        {
            var stamp = DateTimeToShortStampOfNow();
            if (stamp == _lastShortStamp)
            {
                Thread.Sleep(1);
                stamp = DateTimeToShortStampOfNow();
            }

            _lastShortStamp = stamp;
            return stamp;
        }

        public static string AddLongTimeStampSuffix(string content)
        {
            return $"{content}{GetValidLongTimeStamp()}";
        }
        public static string AddShortTimeStampSuffix(string content)
        {
            return $"{content}{GetValidShortTimeStamp()}";
        }

        public static string AddLongTimeStampPrefix(string content)
        {
            return $"{GetValidLongTimeStamp()}{content}";
        }

        public static long DateTimeToLongStampOfNow()
        {
            return DateTimeToLongStamp(DateTime.Now);
        }


        public static long DateTimeToShortStampOfNow()
        {
            return DateTimeToShortStamp(DateTime.Now);
        }
        public static string AddTimeStampForDebug(string content)
        {
#if DEBUG
            return AddShortTimeStampSuffix(content);
#endif
            return content;
        }
    }
}