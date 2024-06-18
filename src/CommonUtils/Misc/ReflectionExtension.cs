using System;
using System.Linq;
using System.Reflection;
using NLog;

namespace CommonUtils.Misc
{
    public static class ReflectionExtension
    {
        public static OperationResult<ConstructorInfo> GetConstructorInfo<TToConstruct>(Type[] parameterTypes)
        {
            try
            {
                var returnValue = typeof(TToConstruct).GetConstructor(parameterTypes);
                return returnValue != null
                    ? OperationResult<ConstructorInfo>.Success(returnValue)
                    : OperationResult<ConstructorInfo>.Failure(
                        ExceptionMessage.NullConstructor<TToConstruct>(parameterTypes));
            }
            catch (Exception e)
            {
                return OperationResult<ConstructorInfo>.Failure(e.Message);
            }
        }


        public static OperationResult<T> CreateInstance<T>(object[] parameterValues)
        {
            if (parameterValues.IsNullOrEmpty()) return OperationResult<T>.Failure(ExceptionMessage.NullConstructorParameter());
            var types = parameterValues.Select(obj => obj.GetType()).ToArray();
            var result = GetConstructorInfo<T>(types);
            if (!result.IsSuccess) return OperationResult<T>.Failure(result.ErrorMessage);
            ;
            try
            {
                var constructorInfo = result.ReturnValue;
                var returnValue = (T)constructorInfo.Invoke(parameterValues);
                return OperationResult<T>.Success(returnValue);
            }
            catch (Exception e)
            {
                return OperationResult<T>.Failure(e.Message);
            }
        }

        public static OperationResult<PropertyInfo> TryGetPropertyOfSpecificType<T>(this object obj,
            string propertyName)
        {
            try
            {
                var type = obj.GetType();
                var property = type.GetProperty(propertyName, typeof(T));
                return property != null
                    ? OperationResult<PropertyInfo>.Success(property)
                    : OperationResult<PropertyInfo>.Failure(ExceptionMessage.InvalidProperty<T>(obj, propertyName));
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e);
                return OperationResult<PropertyInfo>.Failure(ExceptionMessage.UnExpexctedError(e));
            }
        }

        public static OperationResult<T> GetObjectPropertyValue<T>(this object obj, string propertyname)
        {
            try
            {
                var resultProperty = obj.TryGetPropertyOfSpecificType<T>(propertyname);
                if (!resultProperty.IsSuccess) return OperationResult<T>.Failure(resultProperty.ErrorMessage);
                var value = resultProperty.ReturnValue.GetValue(obj, null);
                if (value == null || value.GetType() != typeof(T))
                    return OperationResult<T>.Failure(ExceptionMessage.InvalidProperty<T>(obj, propertyname));
                return OperationResult<T>.Success((T)value);
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e);
                return OperationResult<T>.Failure(ExceptionMessage.UnExpexctedError(e));
            }
        }

        public static OperationResult<VoidValue> SetObjectPropertyValue<T>(this object obj, string propertyname,
            T value)
        {
            try
            {
                var resultProperty = obj.TryGetPropertyOfSpecificType<T>(propertyname);
                if (!resultProperty.IsSuccess) return OperationResult<VoidValue>.Failure(resultProperty.ErrorMessage);
                if(!resultProperty.ReturnValue.CanWrite) return OperationResult<VoidValue>.Failure(ExceptionMessage.ReadonlyProperty(obj, propertyname));
                resultProperty.ReturnValue.SetValue(obj, value);
                return OperationResult<VoidValue>.Success();
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e);
                return OperationResult<VoidValue>.Failure(ExceptionMessage.UnExpexctedError(e));
            }
        }

        public static OperationResult<MethodInfo> TryGeMethodOfSpecificType<T>(this object obj,
            string methodName, Type[] parameterTypes)
        {
            try
            {
                var type = obj.GetType();
                var method = type.GetMethod(methodName, parameterTypes);
                return method != null && method.ReturnType == typeof(T)
                    ? OperationResult<MethodInfo>.Success(method)
                    : OperationResult<MethodInfo>.Failure(ExceptionMessage.InvalidMethod<T>(obj, methodName));
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e);
                return OperationResult<MethodInfo>.Failure(ExceptionMessage.UnExpexctedError(e));
            }
        }
        public static OperationResult<T> MethodInvoke<T>(this object obj, string methodName, params object[] parameterValues)
        {
            try
            {
                Type[]parameterTypes=parameterValues.Length>0 ? parameterValues.Select(p => p.GetType()).ToArray():Type.EmptyTypes;
                var resultMethod =
                    obj.TryGeMethodOfSpecificType<T>(methodName, parameterTypes);
                if (!resultMethod.IsSuccess) return OperationResult<T>.Failure(resultMethod.ErrorMessage);
                var returnValue = resultMethod.ReturnValue.Invoke(obj, parameterValues);
                if (returnValue == null || returnValue.GetType() != typeof(T))
                    return OperationResult<T>.Failure(ExceptionMessage.InvalidMethod<T>(obj, methodName));
                return OperationResult<T>.Success((T)returnValue);
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e);
                return OperationResult<T>.Failure(ExceptionMessage.UnExpexctedError(e));
            }
        }
    }
}