#nullable enable
using System;
using Autodesk.AutoCAD.DatabaseServices;

namespace ACADBase
{
    public static class TransactionExtension
    {
        //TODO EXCEPTION HANDLE
        public static ObjectId GetObject<T>(this Transaction transaction, ObjectId objectId, OpenMode mode,
            Func<T, ObjectId>? funcOnDbObject=null)
            where T : DBObject
        {
            if (!objectId.IsValid) throw ArgumentExceptionOfInvalidId._(objectId);
            try
            {
                using var t = (T)transaction.GetObject(objectId, mode);
                return funcOnDbObject?.Invoke(t) ?? objectId;
            }
            catch (InvalidCastException  e)
            {
                throw ArgumentExceptionOfIdReferToWrongType._<T>(objectId);
            }
                
        }
    }

    public class ArgumentExceptionOfInvalidId: ArgumentException
    {
        public ArgumentExceptionOfInvalidId(string message) : base(message)
        {
        }

        public static ArgumentExceptionOfInvalidId _(ObjectId id)
        {
            return new ArgumentExceptionOfInvalidId($"{id} is not a valid ObjectId.");
        }
    }

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