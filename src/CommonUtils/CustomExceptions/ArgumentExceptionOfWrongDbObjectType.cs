using System;

namespace CommonUtils.CustomExceptions
{
    public class ArgumentExceptionOfWrongDbObjectType : ArgumentException
    {
        public ArgumentExceptionOfWrongDbObjectType(string message) : base(message)
        {
        }

        public static ArgumentExceptionOfWrongDbObjectType _(string dbObjectClassName)
        {
            return new ArgumentExceptionOfWrongDbObjectType(
                $"{dbObjectClassName} is not referring to a valid DbObject Class");
        }
    }
}