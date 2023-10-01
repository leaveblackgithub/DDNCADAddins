using System;
using System.Reflection;
using General;

namespace CommonUtils
{
    public static class ReflectionExtension
    {
        public static bool TryGetProperty<T>(this object obj, string propertyName,out PropertyInfo property)
        {
            property = null;
            Type type = obj.GetType();
            property = type.GetProperty(propertyName, typeof(T));
            return property != null;
        }
        public static PropertyInfo MustGetProperty<T>(this object obj,string propertyName)
        {
            PropertyInfo property;
            obj.TryGetProperty<T>(propertyName, out property);
            if (property==null) throw new ArgumentException($"Type {obj.GetType().Name} doesn't contain property {propertyName} of type {typeof(T).Name}");
            return property;
        }

        public static T GetPropertyValue<T>(this object obj, string propertyname)
        {
            return (T)obj.MustGetProperty<T>(propertyname).GetValue(obj, null);
        }

        public static void SetPropertyValue<T>(this object obj, string propertyname, object value)
        {
            obj.MustGetProperty<T>(propertyname).SetValue(obj, value, null);
        }
    }
}