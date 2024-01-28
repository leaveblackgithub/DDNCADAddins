using System;

namespace CommonUtils.CustomExceptions
{
    public class ArgumentExceptionOfInvalidHandle : ArgumentException
    {
        public ArgumentExceptionOfInvalidHandle(string message) : base(message)
        {
        }
        public static ArgumentExceptionOfInvalidHandle _(long handleValueAsLong)
        {
            return new ArgumentExceptionOfInvalidHandle($"{handleValueAsLong} is not a valid Handle Value.");
        }
    }
}