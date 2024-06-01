using System;

namespace CommonUtils.CustomExceptions
{
    public class ArgumentExceptionOfInvalidDwgFile : ArgumentException
    {
        public ArgumentExceptionOfInvalidDwgFile(string message) : base(message)
        {
        }

        public static ArgumentExceptionOfInvalidDwgFile _(string dwgPath)
        {
            return new ArgumentExceptionOfInvalidDwgFile($"Commands need to be executed in Active Document. Drawing file [{dwgPath}] is not active document.");
        }
    }
}