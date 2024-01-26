#nullable enable
using System;
using Autodesk.AutoCAD.DatabaseServices;

namespace ACADBase
{
    public class ArgumentExceptionOfInvalidId : ArgumentException
    {
        public ArgumentExceptionOfInvalidId(string message) : base(message)
        {
        }

        public static ArgumentExceptionOfInvalidId _(ObjectId id)
        {
            return new ArgumentExceptionOfInvalidId($"{id} is not a valid ObjectId.");
        }
    }
}