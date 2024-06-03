using System;
using System.IO;

namespace CommonUtils.CustomExceptions
{
    public class DwgFileNotFoundException : FileNotFoundException
    {
        public DwgFileNotFoundException(string message) : base(message)
        {
        }

        public static DwgFileNotFoundException _(string dwgPath)
        {
            return new DwgFileNotFoundException(CustomeMessage(dwgPath));
        }

        public static string CustomeMessage(string dwgPath)
        {
            return $"Drawing file [{dwgPath}] not found.";
        }
    }
}