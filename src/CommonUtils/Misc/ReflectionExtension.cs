using System;
using System.Reflection;
using CommonUtils.CustomExceptions;
using NLog;

namespace CommonUtils.Misc
{
    public static class ReflectionExtension
    {
        public static bool TryGetPropertyOfSpecificType<T>(this object obj, string propertyName, out PropertyInfo property)
        {
            property = null;
            var type = obj.GetType();
            property = type.GetProperty(propertyName, typeof(T));
            return property != null;
        }

        public static PropertyInfo MustGetProperty<T>(this object obj, string propertyName)
        {
            PropertyInfo property;
            obj.TryGetPropertyOfSpecificType<T>(propertyName, out property);
            if (property != null) return property;
            var argumentException =
                ArgumentExceptionOfInvalidProperty._<T>(obj, propertyName);
            //log exception in commandresult,not in util methods.
            //LogManager.GetCurrentClassLogger().Error(argumentException);
            throw argumentException;

        }

        public static T GetObjectPropertyValue<T>(this object obj, string propertyname)
        {
            return (T)obj.MustGetProperty<T>(propertyname).GetValue(obj, null);
        }

        public static void SetObjectPropertyValue<T>(this object obj, string propertyname, T value)
        {
            obj.MustGetProperty<T>(propertyname).SetValue(obj, value, null);
        }
    }
}