using System;
using System.Linq;
using System.Reflection;

namespace CommonUtils.Misc
{
    public static class ReflectionExtension
    {
        public static FuncResult GetConstructorInfo<T>(Type[] parameterTypes, out ConstructorInfo constructorInfo)
        {
            var result = new FuncResult();
            constructorInfo = typeof(T).GetConstructor(parameterTypes);
            if (constructorInfo == null) return result.Cancel(ExceptionMessage.NullConstructor<T>());

            return result;
        }

        public static FuncResult CreateInstance<T>(object[] parameters, out T instance) where T : new()
        {
            instance = default;
            var result = GetConstructorInfo<T>(parameters.Select(obj => obj.GetType()).ToArray(),
                out var constructorInfo);
            if (result.IsCancel) return result;
            instance = (T)constructorInfo.Invoke(parameters);
            if (instance == null) return result.Cancel();
            return result;
        }

        public static bool TryGetPropertyOfSpecificType<T>(this object obj, string propertyName,
            out PropertyInfo property)
        {
            property = null;
            var type = obj.GetType();
            property = type.GetProperty(propertyName, typeof(T));
            return property != null;
        }

        public static FuncResult MustGetProperty<T>(this object obj, string propertyName, out PropertyInfo propertyInfo)
        {
            var result = new FuncResult();
            propertyInfo = default;
            obj.TryGetPropertyOfSpecificType<T>(propertyName, out propertyInfo);
            if (propertyInfo != null) return result;
            return result.Cancel(ExceptionMessage.InvalidProperty<T>(obj, propertyName));
        }

        public static FuncResult GetObjectPropertyValue<T>(this object obj, string propertyname, out T value)
        {
            value = default;
            var result = obj.MustGetProperty<T>(propertyname, out var propertyInfo);
            if (result.IsCancel) return result;
            value = (T)propertyInfo.GetValue(obj, null);
            return result;
        }

        public static FuncResult SetObjectPropertyValue<T>(this object obj, string propertyname, T value)
        {
            var result = obj.MustGetProperty<T>(propertyname, out var propertyInfo);
            if (result.IsCancel) return result;
            propertyInfo.SetValue(obj, value, null);
            return result;
        }
    }
}