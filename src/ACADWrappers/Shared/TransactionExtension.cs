using System;
using Autodesk.AutoCAD.DatabaseServices;

namespace ACADWrappers.Shared
{
    public static class TransactionExtension
    {
        public static void ReadDbObject<T>(this Transaction transaction, ObjectId objectId, Action<T> action)
            where T : DBObject
        {
            ArgumentException argumentException = new ArgumentException(
                $"{objectId.ToString()} is not a valid DbObject of type {typeof(T).Name}");
            if (!objectId.IsValid) throw argumentException;
            using (var dbObject = transaction.GetObject(objectId, OpenMode.ForRead))
            {
                var t = dbObject as T;
                if (dbObject == null || t is null)
                {
                    throw argumentException;
                }

                action(t);
            }
        }
        public static void ReadDbObject<T>(this Transaction transaction, IntPtr objectIdIntPtr, Action<T> action)
            where T : DBObject
        {
            ObjectId objectId=new ObjectId(objectIdIntPtr);
           transaction.ReadDbObject<T>(objectId,action);
        }
    }
}