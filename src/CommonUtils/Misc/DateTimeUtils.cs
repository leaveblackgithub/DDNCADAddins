using System;

namespace CommonUtils.Misc
{
    public static class DateTimeUtils
    {
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
            return (long)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0,0, DateTimeKind.Utc))
                .TotalMilliseconds;
        }

        public static string AddTimeStampSuffix(string content)
        {
            return content + DateTimeToLongTimeStamp(DateTime.Now);
        }
        public static string AddTimeStampPrefix(string content)
        {
            return $"{DateTimeToLongTimeStamp(DateTime.Now)}{content}";
        }

        public static long DateTimeToLongTimeStampOfNow()
        {
            return DateTimeUtils.DateTimeToLongTimeStamp(DateTime.Now);
        }
    }
}