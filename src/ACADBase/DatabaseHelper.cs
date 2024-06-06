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

        public static CommandResult NewDatabaseHelper<T>(string drawingFile,
            IMessageProvider messageProvider,out T newDataBaseHelper) where T : DatabaseHelper, new()
        {
            newDataBaseHelper = default(T);
            var result=ReflectionExtension.CreateInstance<T>(new object[] { drawingFile, messageProvider },out newDataBaseHelper);
            if (newDataBaseHelper == null||newDataBaseHelper.IsInvalid) return result.Cancel(NullReferenceExceptionOfDatabase.CustomeMessage(drawingFile));
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

        public CommandResult RunFuncInTransaction<T>(HandleValue handleValue,
            params Func<T, CommandResult>[] funcs) where T : DBObject
        {
            var result = new CommandResult();
            if (funcs.IsNullOrEmpty()) return result;
            using var tr = NewTransactionHelper();
            
            if (!TryGetObjectId(handleValue, out var objectId))
                return result.Cancel(ArgumentExceptionOfInvalidHandle._(handleValue.HandleAsLong));
            result = tr.RunFuncsOnObject(objectId, funcs);
            tr.Commit();
            return result;
            
        }

        public CommandResult CreateInCurrentSpace<T>(out HandleValue handleValue,
            params Func<T, CommandResult>[] funcs)
            where T : Entity, new()
        {
            handleValue = null;
            using var tr = NewTransactionHelper();
            handleValue = tr.CreateObject<T>(CadDatabase.CurrentSpaceId);
            tr.Commit();
            CommandResult result = RunFuncInTransaction(handleValue, funcs);
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

        public virtual bool TryGetObjectId(HandleValue handleValue, out ObjectId objectId)
        {
            return CadDatabase.TryGetObjectId(handleValue.ToHandle(), out objectId);
        }

    }
}