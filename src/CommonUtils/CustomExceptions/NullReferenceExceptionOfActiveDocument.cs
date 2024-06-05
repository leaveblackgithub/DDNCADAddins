using System;

namespace CommonUtils.CustomExceptions
{
    public class NullReferenceExceptionOfActiveDocument : NullReferenceException
    {
        //default constructor
        public NullReferenceExceptionOfActiveDocument  (string message) : base(message)
        {
        }

        public static NullReferenceExceptionOfActiveDocument _()
        {
            return new NullReferenceExceptionOfActiveDocument($"Active Document is null.");
        }
    }
}