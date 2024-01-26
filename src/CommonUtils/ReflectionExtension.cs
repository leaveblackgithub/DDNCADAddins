using System;
using System.Reflection;
using NLog;

namespace CommonUtils
{
    public static class ReflectionExtension
    {
        public static bool TryGetProperty<T>(this object obj, string propertyName, out PropertyInfo property)
        {
            property = null;
            var type = obj.GetType();
            property = type.GetProperty(propertyName, typeof(T));
            return property != null;
        }

        public static PropertyInfo MustGetProperty<T>(this object obj, string propertyName)
        {
            PropertyInfo property;
            obj.TryGetProperty<T>(propertyName, out property);
            if (property == null)
            {
                var argumentException =
                    new ArgumentException(
                        $"Type {obj.GetType().Name} doesn't contain property {propertyName} of type {typeof(T).Name}");
                LogManager.GetCurrentClassLogger().Error(argumentException);
                throw argumentException;
            }

            return property;
        }

        public static T GetObjectPropertyValue<T>(this object obj, string propertyname)
        {
            return (T)obj.MustGetProperty<T>(propertyname).GetValue(obj, null);
        }

        public static void SetObjectPropertyValue<T>(this object obj, string propertyname, object value)
        {
            obj.MustGetProperty<T>(propertyname).SetValue(obj, value, null);
        }
    }
}