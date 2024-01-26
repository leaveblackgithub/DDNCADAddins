#nullable enable
using System;
using Autodesk.AutoCAD.DatabaseServices;
using NLog;

namespace ACADBase
{
    public static class DatabaseExtension
    {
        public static ObjectId RunFuncInTransaction(this Database database, Func<Transaction, ObjectId> func)
        {
            using (var tr = database.TransactionManager.StartTransaction())
            {
                try
                {
                    return func(tr);
                }
                catch (Exception e)
                {
                    LogManager.GetCurrentClassLogger().Error(e);
                    throw;
                }
                finally
                {
                    tr.Commit();
                }
            }
        }

        public static ObjectId CreateInModelSpace<T>(this Database database, Action<T>? action = null)
            where T : Entity, new()
        {
            var databaseBlockTableId = database.BlockTableId;
            return database.RunFuncInTransaction(tr =>
            {
                var obj = new T();
                obj.SetDatabaseDefaults();
                action?.Invoke(obj);
                var modelSpaceId = tr.GetObject<BlockTable>(databaseBlockTableId, OpenMode.ForRead,
                    blockTable => blockTable[BlockTableRecord.ModelSpace]);
                var resultObjId = tr.GetObject<BlockTableRecord>(modelSpaceId, OpenMode.ForWrite,
                    modelSpace => modelSpace.AppendEntity(obj));
                tr.AddNewlyCreatedDBObject(obj, true);
                return resultObjId;
            });
        }

        public static string GetDwgName(this Database database)
        {
            return database.OriginalFileName;
        }
    }
}