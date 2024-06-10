using System;
using CommonUtils.Misc;

namespace CommonUtils.CustomExceptions
{
    public class ArgumentExceptionOfInvalidDwgFile : ArgumentException
    {
        public ArgumentExceptionOfInvalidDwgFile(string message) : base(message)
        {
        }

        public static ArgumentExceptionOfInvalidDwgFile _(string dwgPath)
        {
            return new ArgumentExceptionOfInvalidDwgFile(CustomeMessage(dwgPath));
        }

        public static string CustomeMessage(string dwgPath)
        {
            return ExceptionMessage.NullActiveDocument(dwgPath);
        }
    }
}