using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CADAddins.General
{

    public static class GenUtils
    {
        public static long DateTimeToTimeStamp(DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }
        /// <summary>
        /// DateTime转换为13位时间戳（单位：毫秒）
        /// </summary>
        /// <param name="dateTime"> DateTime</param>
        /// <returns>13位时间戳（单位：毫秒）</returns>
        public static long DateTimeToLongTimeStamp(DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        public static string AddTimeStampSuffix(string content)
        {
            return content + DateTimeToLongTimeStamp(DateTime.Now).ToString();
        }
        public static bool HasBoundPrefix(string name)
        {
            Regex regex=new Regex(@"\$\d\$");
            return regex.IsMatch(name);
        }
        public static string RemoveBoundPrefix(string name)
        {
            if (!HasBoundPrefix(name)) return name;
            Regex regex = new Regex(@"\$\d\$");
            string[] result = regex.Split(name);
            return result.Last();
        }
        public static bool ContainProperty(this object instance, string propertyName)
        {
            if (instance != null && !string.IsNullOrEmpty(propertyName))
            {
                PropertyInfo _findedPropertyInfo = instance.GetType().GetProperty(propertyName);
                return (_findedPropertyInfo != null);
            }
            return false;
        }
        public static object GetObjectPropertyValue<T>(T t, string propertyname)
        {
            Type type = typeof(T);
            PropertyInfo property = type.GetProperty(propertyname);
            if (property == null)
            {
                return null;
            }
            object o = property.GetValue(t, null);
            return o;
        }
        public static bool SetObjectPropertyValue<T>(T t, string propertyname,object value)
        {
            Type type = typeof(T);
            PropertyInfo property = type.GetProperty(propertyname);
            if (property == null)
            {
                return false;
            }
            property.SetValue(t, value);
            return true;
        }
    }
}
