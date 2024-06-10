using System;
using System.Linq;
using System.Reflection;
using CommonUtils.CustomExceptions;

namespace CommonUtils.Misc
{
    public static class ReflectionExtension
    {
        // public static FuncResult GetConstructorInfo<T>(Type[] parameterTypes,out ConstructorInfo constructorInfo)
        // {
        //     FuncResult result=new FuncResult();
        //     constructorInfo= typeof(T).GetConstructor(parameterTypes);
        //     if (constructorInfo == null)
        //     {
        //         return result.Cancel(NullReferenceExceptionOfConstructor.CustomMessage<T>());
        //     }
        //
        //     return result;
        // }
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
        //
        // public static FuncResult CreateInstance<T>(object[] parameterValues, out T t)
        // {
        //     t= default(T);
        //     var result = GetConstructorInfo<T>(parameterValues.Select(obj => obj.GetType()).ToArray(),
        //         out var constructorInfo);
        //     if (result.IsCancel) return result;
        //     try
        //     {
        //         t = (T)constructorInfo.Invoke(parameterValues);
        //
        //     }
        //     catch (Exception e)
        //     {
        //         result.Cancel(e.Message);
        //
        //     }
        //     return result;
        // }

        public static OperationResult<T> CreateInstance<T>(object[] parameterValues)
        {
            if (parameterValues == null) return OperationResult<T>.Failure(ExceptionMessage.NullConstructorParameter());
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

        public static bool TryGetPropertyOfSpecificType<T>(this object obj, string propertyName,
            out PropertyInfo property)
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