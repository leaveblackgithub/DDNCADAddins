using System;
using CommonUtils.Misc;

namespace CommonUtils.CustomExceptions
{
    public class NullReferenceExceptionOfConstructor : NullReferenceException
    {
        //default constructor
        public NullReferenceExceptionOfConstructor(string message) : base(message)
        {
        }

        public static NullReferenceExceptionOfConstructor _<T>(Type[] parameterTypes)
        {
            return new NullReferenceExceptionOfConstructor(CustomMessage<T>(parameterTypes));
        }

        public static string CustomMessage<T>(Type[] parameterTypes)
        {
            return ExceptionMessage.NullConstructor<T>(parameterTypes);
        }
    }
}