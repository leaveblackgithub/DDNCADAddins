using System;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils;
using NLog;

namespace ACADBase
{
    public class TransactionHelper : DisposableClass
    {
        public TransactionHelper(Transaction transaction)
        {
            ActiveTransaction = transaction;
        }

        public Transaction ActiveTransaction { get; set; }

        public T GetObject<T>(ObjectId objectId, OpenMode mode)
            where T : DBObject
        {
            if (!objectId.IsValid) throw ArgumentExceptionOfInvalidId._(objectId);
            try
            {
                var t = (T)ActiveTransaction.GetObject(objectId, mode);
                return t;
            }
            catch (InvalidCastException e)
            {
                LogManager.GetCurrentClassLogger().Error(e);
                throw ArgumentExceptionOfIdReferToWrongType._<T>(objectId);
            }
        }

        public HandleValue CreateInModelSpace<T>(ObjectId databaseBlockTableId)
            where T : Entity, new()
        {
            using var obj = new T();
            obj.SetDatabaseDefaults();
            using var blockTable = GetObject<BlockTable>(databaseBlockTableId, OpenMode.ForRead);
            using var modelSpace =
                GetObject<BlockTableRecord>(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
            modelSpace.AppendEntity(obj);
            AddNewlyCreatedDBObject(obj, true);
            return HandleValue.FromObject(obj);
        }

        public CommandResult RunFuncsOnObject<T>(ObjectId objectId, Func<T, CommandResult>[] funcs) where T : DBObject
        {
            var result = new CommandResult();
            if (funcs.IsNullOrEmpty()) return result;
            var obj = GetObject<T>(objectId, OpenMode.ForWrite);
            result = funcs.RunForEach(obj);
            return result;
        }
        
        public void Commit()
        {
            ActiveTransaction?.Commit();
        }

        protected override void DisposeUnManaged()
        {
        }

        protected override void DisposeManaged()
        {
            Commit();
            ActiveTransaction?.Dispose();
        }

        public void AddNewlyCreatedDBObject(DBObject obj, bool add)
        {
            ActiveTransaction.AddNewlyCreatedDBObject(obj, add);
        }
        
    }
}