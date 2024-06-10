using System;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.CustomExceptions;
using CommonUtils.Misc;
using NLog;

namespace ACADBase
{
    public class TransactionHelper : DisposableClass, ITransactionHelper
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

        public HandleValue CreateObject<T>(ObjectId modelSpaceId)
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

        public OperationResult<VoidValue> RunFuncsOnObject<T>(ObjectId objectId,
            Func<T, OperationResult<VoidValue>>[] funcs)
            where T : DBObject
        {
            if (funcs.IsNullOrEmpty()) return OperationResult<VoidValue>.Success();
            using var obj = GetObject<T>(objectId, OpenMode.ForWrite);
            return funcs.RunForEach(obj);
        }

        public void Commit()
        {
            ActiveTransaction?.Commit();
        }

        public void AddNewlyCreatedDBObject(DBObject obj, bool add)
        {
            ActiveTransaction.AddNewlyCreatedDBObject(obj, add);
        }

        protected override void DisposeUnManaged()
        {
            ActiveTransaction?.Dispose();
            ActiveTransaction = null;
        }

        protected override void DisposeManaged()
        {
        }
    }
}