using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.Misc;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace ACADBase
{
    public  abstract class DatabaseHelper : DisposableClass, IDatabaseHelper
    {
        protected IMessageProvider FldMsgProvider;
        private Database _cadDatabase;

        public static FuncResult NewDatabaseHelper<T>(string drawingFile,
            IMessageProvider messageProvider,out T newDataBaseHelper) where T : DatabaseHelper, new()
        {
            newDataBaseHelper = null;
            FuncResult result = new FuncResult();
            result = ReflectionExtension.GetConstructorInfo<T>
                (new Type[] { typeof(string), typeof(IMessageProvider) }, out var constructorInfo);
            if (result.IsCancel) return result;
            newDataBaseHelper = (T)constructorInfo.Invoke(new object[] { drawingFile, messageProvider });
            if(newDataBaseHelper == null||newDataBaseHelper.IsInvalid) return result.Cancel(ExceptionMessage.NullDatabase(drawingFile));
            return result;
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

            if (HostApplicationServiceWrapper.IsTargetDrawingActive(drawingFile))
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

        public FuncResult RunFuncInTransaction<T>(HandleValue handleValue,
            params Func<T, FuncResult>[] funcs) where T : DBObject
        {
            var result = new FuncResult();
            if (funcs.IsNullOrEmpty()) return result;
            using var tr = NewTransactionHelper();

            result = TryGetObjectId(handleValue, out var objectId);
            if (result.IsCancel) return result;
            result = tr.RunFuncsOnObject(objectId, funcs);
            tr.Commit();
            return result;
            
        }

        public FuncResult CreateInCurrentSpace<T>(out HandleValue handleValue)
            where T : Entity, new()
        {
            handleValue = null;
            using var tr = NewTransactionHelper();
            FuncResult result = tr.CreateObjectInModelSpace<T>(CadDatabase.CurrentSpaceId,out handleValue);
            tr.Commit();
            return result;
        }

        private ObjectId GetBlockTableId<T>() where T : Entity, new()
        {
            return CadDatabase.BlockTableId;
        }

        public ITransactionHelper NewTransactionHelper()

        {
            return new TransactionHelper(CadDatabase.TransactionManager.StartTransaction());
        }

        public virtual FuncResult TryGetObjectId(HandleValue handleValue, out ObjectId objectId)
        {
            objectId = default(ObjectId);
            FuncResult result = handleValue.ToHandle(out var handle);
            if(result.IsCancel)return result;
            if (CadDatabase.TryGetObjectId(handle, out objectId)) return result;
            else return result.Cancel(ExceptionMessage.InvalidHandle(handleValue.HandleAsLong));
        }

    }
}