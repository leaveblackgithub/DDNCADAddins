using System;

namespace CommonUtils.CustomExceptions
{
    public class NullReferenceExceptionOfDatabase : NullReferenceException
    {
        //default constructor
        public NullReferenceExceptionOfDatabase(string message) : base(message)
        {
        }

        public static NullReferenceExceptionOfDatabase _(string dwgPath)
        {
            return new NullReferenceExceptionOfDatabase(CustomeMessage(dwgPath));
        }
        public static string CustomeMessage(string dwgPath)
        {
            return $"DwgDatabase read from [{dwgPath}] is null.";
        }
    }
}