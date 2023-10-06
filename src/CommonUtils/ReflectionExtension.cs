namespace CommonUtils
{
    public static class ReflectionExtension
    {
        public static bool ContainProperty(this object instance, string propertyName)
        {
            if (instance != null && !string.IsNullOrEmpty(propertyName))
            {
                var _findedPropertyInfo = instance.GetType().GetProperty(propertyName);
                return _findedPropertyInfo != null;
            }

            return false;
        }

        public static object GetObjectPropertyValue<T>(T t, string propertyname)
        {
            var type = typeof(T);
            var property = type.GetProperty(propertyname);
            if (property == null) return null;
            var o = property.GetValue(t, null);
            return o;
        }

        public static bool SetObjectPropertyValue<T>(T t, string propertyname, object value)
        {
            var type = typeof(T);
            var property = type.GetProperty(propertyname);
            if (property == null) return false;
            property.SetValue(t, value);
            return true;
        }
    }
}