#nullable enable
using System;
using Autodesk.AutoCAD.DatabaseServices;
using NLog;

namespace ACADBase
{
    public static class TransactionExtension
    {
        //TODO EXCEPTION HANDLE
        public static ObjectId GetObject<T>(this Transaction transaction, ObjectId objectId, OpenMode mode,
            Func<T, ObjectId>? funcOnDbObject = null)
            where T : DBObject
        {
            if (!objectId.IsValid) throw ArgumentExceptionOfInvalidId._(objectId);
            try
            {
                using var t = (T)transaction.GetObject(objectId, mode);
                return funcOnDbObject?.Invoke(t) ?? objectId;
            }
            catch (InvalidCastException e)
            {
                LogManager.GetCurrentClassLogger().Error(e);
                throw ArgumentExceptionOfIdReferToWrongType._<T>(objectId);
            }
        }
    }
}