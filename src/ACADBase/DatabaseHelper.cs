using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.CustomExceptions;
using CommonUtils.Misc;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace ACADBase
{
    public  abstract class DatabaseHelper : DisposableClass, IDatabaseHelper
    {
        protected IMessageProvider FldMsgProvider;
        private Database _cadDatabase;

        public static OperationResult<IDatabaseHelper> NewDatabaseHelper<T>(string drawingFile,
            IMessageProvider messageProvider) where T : DatabaseHelper, new()
        {
            var result=ReflectionExtension.CreateInstance<T>(new object[] { drawingFile, messageProvider });
            if (!result.IsSuccess) return OperationResult<IDatabaseHelper>.Failure(result.ErrorMessage);
            T newDataBaseHelper = result.ReturnValue;
            if (newDataBaseHelper == null||newDataBaseHelper.IsInvalid) return OperationResult<IDatabaseHelper>.Failure(NullReferenceExceptionOfDatabase.CustomeMessage(drawingFile));
            return OperationResult<IDatabaseHelper>.Success(newDataBaseHelper);
        }

        public DatabaseHelper()
        {

        }
        public DatabaseHelper(string drawingFile="", IMessageProvider messageProvider = null)
        {
            CadDatabase = GetDatabase(drawingFile);
            IsInvalid = (CadDatabase == null);
            if (IsInvalid) return;
            ActiveMsgProvider = messageProvider;
        }

        public bool IsInvalid { get; set; }

        protected Database GetCurrentDatabase()
        {
            return HostApplicationServiceWrapper.GetWorkingDatabase();
        }
        protected Database GetDatabase(string drawingFile)
        {
            Database database = null;

            if (HostApplicationServiceWrapper.IsTargetDrawingActive(drawingFile).IsSuccess)
            {
                database = GetCurrentDatabase();
            }
            else
            {
                database = DatabaseExtension.GetDwgDatabase(drawingFile);
            }
            return database;

        }

        public Database CadDatabase
        {
            get => _cadDatabase;
            protected set
            {
                _cadDatabase = value;
            }
        }

        public abstract IMessageProvider ActiveMsgProvider { get; set; }
        public void WriteMessage(string message)
        {
            ActiveMsgProvider.Show(message);
        }

        public void ShowError(Exception exception)
        {
            ActiveMsgProvider.Error(exception);
        }
        protected override void DisposeUnManaged()
        {
            CadDatabase=null;
            ActiveMsgProvider = null;
        }

        protected override void DisposeManaged()
        {
        }

        public OperationResult<VoidValue> RunFuncInTransaction<T>(HandleValue handleValue,
            params Func<T, OperationResult<VoidValue>>[] funcs) 
            where T : DBObject

        {
            var result = OperationResult<VoidValue>.Success();
            if (funcs.IsNullOrEmpty()) return result;
            var tr = NewTransactionHelper();
            var resultObjectId = TryGetObjectId(handleValue);
            result = resultObjectId.Then(()=>tr.RunFuncsOnObject(resultObjectId.ReturnValue, funcs));
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

        private ObjectId GetBlockTableId<T>() where T : Entity, new()
        {
            return CadDatabase.BlockTableId;
        }

        public ITransactionHelper NewTransactionHelper()

        {
            return new TransactionHelper(CadDatabase.TransactionManager.StartTransaction());
        }

        public virtual OperationResult<ObjectId> TryGetObjectId(HandleValue handleValue)
        {
            if( CadDatabase.TryGetObjectId(handleValue.ToHandle(), out var objectId))
                return OperationResult<ObjectId>.Success(objectId);
            return OperationResult<ObjectId>.Failure(ExceptionMessage.InvalidHandle(handleValue.HandleAsLong));
        }

    }
}