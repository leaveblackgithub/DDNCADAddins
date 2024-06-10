using System;

namespace CommonUtils.CustomExceptions
{
    public class ArgumentExceptionOfInvalidProperty : ArgumentException
    {
        public ArgumentExceptionOfInvalidProperty(string message) : base(message)
        {
        }

        public static ArgumentExceptionOfInvalidProperty _<T>(object obj, string propertyName)
        {
            return new ArgumentExceptionOfInvalidProperty(
                $"Type {obj.GetType().Name} doesn't contain property {propertyName} of type {typeof(T).Name}");
        }
    }
}