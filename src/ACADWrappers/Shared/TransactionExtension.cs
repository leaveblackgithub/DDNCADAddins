using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using NLog;
using Exception = System.Exception;

namespace ACADWrappers.Shared
{
    public static class TransactionExtension
    {
        public static void ReadDbObject<T>(this Transaction transaction, ObjectId objectId, Action<T> action)
            where T : DBObject
        {
            var argumentException = new ArgumentException(
                $"{objectId.ToString()} is not a valid ObjectId for DbObject of type {typeof(T).Name}");
            try
            {
                using (var dbObject = transaction.GetObject(objectId, OpenMode.ForRead))
                {
                    var t = dbObject as T;
                    if (dbObject == null || t is null) return;
                    action(t);
                    argumentException = null;
                }
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e);
            }
            finally
            {
                if (argumentException != null)
                {
                    LogManager.GetCurrentClassLogger().Error(argumentException);
                    throw argumentException;
                }
            }
        }

        public static void ReadDbObject<T>(this Transaction transaction, IntPtr objectIdIntPtr, Action<T> action)
            where T : DBObject
        {
            var objectId = new ObjectId(objectIdIntPtr);
            transaction.ReadDbObject(objectId, action);
        }
    }
}