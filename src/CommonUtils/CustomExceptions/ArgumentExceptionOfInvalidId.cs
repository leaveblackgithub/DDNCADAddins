using System;

namespace CommonUtils.CustomExceptions
{
    public class ArgumentExceptionOfInvalidId : ArgumentException
    {
        public ArgumentExceptionOfInvalidId(string message) : base(message)
        {
        }

        public static ArgumentExceptionOfInvalidId _(string idString)
        {
            return new ArgumentExceptionOfInvalidId($"{idString} is not a valid ObjectId.");
        }
    }
}