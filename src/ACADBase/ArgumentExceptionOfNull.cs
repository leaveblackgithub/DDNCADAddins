using System;

namespace ACADBase
{
    public class ArgumentExceptionOfNull:Exception
    {
        //default constructor
        public ArgumentExceptionOfNull(string message) : base(message)
        {
        }
    }
}