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
            return new NullReferenceExceptionOfConstructor(CustomMessage<T>());
        }

        public static string CustomMessage<T>()
        {
            return $"Class [{typeof(T)}] doesn't have ";
        }
    }
}