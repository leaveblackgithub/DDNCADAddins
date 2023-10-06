#nullable enable
using System;
using Autodesk.AutoCAD.DatabaseServices;

namespace ACADBase
{
    public class ArgumentExceptionOfIdReferToWrongType : ArgumentException
    {
        public ArgumentExceptionOfIdReferToWrongType(string message) : base(message)
        {
        }

        public static ArgumentExceptionOfIdReferToWrongType _<T>(ObjectId id)
        {
            return new ArgumentExceptionOfIdReferToWrongType($"{id} is not referring to a DbObject of Type {typeof(T).Name}.");
        }
    }
}