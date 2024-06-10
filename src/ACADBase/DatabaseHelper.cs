using System;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.Misc;

namespace ACADBase
{
    public abstract class DatabaseHelper : DisposableClass, IDatabaseHelper
    {
        public DatabaseHelper()
        {
        }

        public DatabaseHelper(string drawingFile = "")
        {
            CadDatabase = GetDatabase(drawingFile);
            IsInvalid = CadDatabase == null;
            if (IsInvalid) return;
        }

        public bool IsInvalid { get; set; }

        public Database CadDatabase { get; protected set; }


        public OperationResult<VoidValue> RunFuncInTransaction<T>(HandleValue handleValue,
            params Func<T, OperationResult<VoidValue>>[] funcs)
            where T : DBObject

        {
            var result = OperationResult<VoidValue>.Success();
            if (funcs.IsNullOrEmpty()) return result;
            var tr = NewTransactionHelper();
            var resultObjectId = TryGetObjectId(handleValue);
            result = resultObjectId.Then(() => tr.RunFuncsOnObject(resultObjectId.ReturnValue, funcs));
            tr.Commit();
            tr.Dispose();
            return result;
        }

        public OperationResult<HandleValue> CreateInCurrentSpace<T>() where T : Entity, new()
        {
            using var tr = NewTransactionHelper();
            var handleValue = tr.CreateObject<T>(CadDatabase.CurrentSpaceId);
            tr.Commit();
            return OperationResult<HandleValue>.Success(handleValue);
        }

        public virtual OperationResult<ObjectId> TryGetObjectId(HandleValue handleValue)
        {
            if (CadDatabase.TryGetObjectId(handleValue.ToHandle(), out var objectId))
                return OperationResult<ObjectId>.Success(objectId);
            return OperationResult<ObjectId>.Failure(ExceptionMessage.InvalidHandle(handleValue.HandleAsLong));
        }

        public static OperationResult<IDatabaseHelper> NewDatabaseHelper<T>(string drawingFile)
            where T : DatabaseHelper, new()
        {
            var result = ReflectionExtension.CreateInstance<T>(new object[] { drawingFile });
            if (!result.IsSuccess) return OperationResult<IDatabaseHelper>.Failure(result.ErrorMessage);
            var newDataBaseHelper = result.ReturnValue;
            if (newDataBaseHelper == null || newDataBaseHelper.IsInvalid)
                return OperationResult<IDatabaseHelper>.Failure(ExceptionMessage.NullDatabase(drawingFile));
            return OperationResult<IDatabaseHelper>.Success(newDataBaseHelper);
        }

        protected Database GetCurrentDatabase()
        {
            return HostApplicationServiceWrapper.GetWorkingDatabase();
        }

        protected Database GetDatabase(string drawingFile)
        {
            Database database = null;

            if (HostApplicationServiceWrapper.IsTargetDrawingActive(drawingFile).IsSuccess)
                database = GetCurrentDatabase();
            else
                database = DatabaseExtension.GetDwgDatabase(drawingFile);
            return database;
        }


        protected override void DisposeUnManaged()
        {
            CadDatabase = null;
        }

        protected override void DisposeManaged()
        {
        }

        private ObjectId GetBlockTableId<T>() where T : Entity, new()
        {
            return CadDatabase.BlockTableId;
        }

        public ITransactionHelper NewTransactionHelper()

        {
            return new TransactionHelper(CadDatabase.TransactionManager.StartTransaction());
        }
    }
}