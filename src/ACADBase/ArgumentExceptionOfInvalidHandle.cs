using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace ACADBase
{
    public class ArgumentExceptionOfInvalidHandle : Exception
    {
        public ArgumentExceptionOfInvalidHandle(string message) : base(message)
        {
        }
        public static ArgumentExceptionOfInvalidHandle _(Handle handle)
        {
            return new ArgumentExceptionOfInvalidHandle($"{handle.Value} is not a valid Handle.");
        }
        public static ArgumentExceptionOfInvalidHandle _(HandleValue handleValue)
        {
            return new ArgumentExceptionOfInvalidHandle($"{handleValue.HandleAsLong} is not a valid Handle Value.");
        }
        public static ArgumentExceptionOfInvalidHandle _(long handleValueAsLong)
        {
            return new ArgumentExceptionOfInvalidHandle($"{handleValueAsLong} can not be converted to a Handle.");
        }
    }
}