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
            return new NullReferenceExceptionOfDatabase($"dwgDatabase read from [{dwgPath}] is null.");
        }
    }
}