using System;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils;
using CommonUtils.CustomExceptions;
using CommonUtils.Misc;
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
            if (!objectId.IsValid) throw ArgumentExceptionOfInvalidId._(objectId.ToString());
            try
            {
                var t = (T)ActiveTransaction.GetObject(objectId, mode);
                return t;
            }
            catch (InvalidCastException e)
            {
                LogManager.GetCurrentClassLogger().Error(e);
                throw ArgumentExceptionOfIdReferToWrongType._<T>(objectId.ToString());
            }
        }

        public HandleValue CreateInModelSpace<T>(ObjectId modelSpaceId)
            where T : Entity, new()
        {
            using var obj = new T();
            using var modelSpace =
                GetObject<BlockTableRecord>(modelSpaceId, OpenMode.ForWrite);
            modelSpace.AppendEntity(obj);
            AddNewlyCreatedDBObject(obj, true);

            obj.SetDatabaseDefaults();
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
            ActiveTransaction?.Dispose();
            ActiveTransaction = null;
        }

        protected override void DisposeManaged()
        {
        }

        public void AddNewlyCreatedDBObject(DBObject obj, bool add)
        {
            ActiveTransaction.AddNewlyCreatedDBObject(obj, add);
        }
        
    }
}