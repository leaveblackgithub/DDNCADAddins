
using System;

namespace CommonUtils.CustomExceptions
{
    public class ArgumentExceptionOfIdReferToWrongType : ArgumentException
    {
        public ArgumentExceptionOfIdReferToWrongType(string message) : base(message)
        {
        }

        public static ArgumentExceptionOfIdReferToWrongType _<T>(string idString)
        {
            return new ArgumentExceptionOfIdReferToWrongType(
                $"{idString} is not referring to a DbObject of Type {typeof(T).Name}.");
        }
    }
}