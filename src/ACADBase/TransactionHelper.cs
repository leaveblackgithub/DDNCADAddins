using System;
using Autodesk.AutoCAD.DatabaseServices;
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

        public FuncResult GetObject<T>(ObjectId objectId, OpenMode mode,out T t)
            where T : DBObject
        {
            t = default(T);
            string idString = objectId.ToString();
            var result=new FuncResult();
            if (!objectId.IsValid)
            {
                return result.Cancel(ExceptionMessage.InvalidId(idString));
            }

            try
            {
                t = (T)ActiveTransaction.GetObject(objectId, mode);
                return result;
            }
            catch (InvalidCastException e)
            {
                LogManager.GetCurrentClassLogger().Error(e);
                return result.Cancel(ExceptionMessage.IdReferToWrongType<T>(idString));
            }
        }

        public FuncResult CreateObjectInModelSpace<T>(ObjectId modelSpaceId,out HandleValue handleValue)
            where T : Entity, new()
        {
            handleValue = default(HandleValue);
            var result = new FuncResult();
            using var obj = new T();
            result=GetObject<BlockTableRecord>(modelSpaceId, OpenMode.ForWrite,out var modelSpace);
            if (result.IsCancel) return result;
            modelSpace.AppendEntity(obj);
            AddNewlyCreatedDBObject(obj, true);

            obj.SetDatabaseDefaults();
            handleValue = HandleValue.FromObject(obj);
            return result;
        }

        public FuncResult RunFuncsOnObject<T>(ObjectId objectId, Func<T, FuncResult>[] funcs) where T : DBObject
        {
            var result = new FuncResult();
            if (funcs.IsNullOrEmpty()) return result;
            result = GetObject<T>(objectId, OpenMode.ForWrite,out var obj);
            if (result.IsCancel) return result;
            result = funcs.RunForEach(obj);
            obj.Dispose();
            return result;
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