using System;

namespace CommonUtils.CustomExceptions
{
    public class NullReferenceExceptionOfConstructor : NullReferenceException
    {
        //default constructor
        public NullReferenceExceptionOfConstructor(string message) : base(message)
        {
        }

        public static NullReferenceExceptionOfConstructor _<T>()
        {
            return new NullReferenceExceptionOfConstructor($"Class [{typeof(T)}] doesn't have ");
        }
    }
}