using System;
using System.IO;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.CustomExceptions;
using CommonUtils.Misc;

namespace ACADBase
{
    public  class DatabaseHelper : DisposableClass, IDatabaseHelper
    {
        private IMessageProvider _messageProvider;
        public DatabaseHelper(Database cadDatabase, IMessageProvider messageProvider = null)
        {
            CadDatabase = cadDatabase;
            ActiveMsgProvider = messageProvider;
        }
        public Database CadDatabase { get; }


        public IMessageProvider ActiveMsgProvider
        {
            get => _messageProvider;
            private set
            {
#if AcConcole
                _messageProvider = value ?? new MessageProviderOfMessageBox();
#else
                _messageProvider =
                    value ?? new MessageProviderOfEditor(Application.DocumentManager.CurrentDocument.Editor);
#endif
            }
        }
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
            CadDatabase?.Dispose();
        }

        protected override void DisposeManaged()
        {
        }

        public CommandResult RunFuncInTransaction<T>(HandleValue handleValue,
            params Func<T, CommandResult>[] funcs) where T : DBObject
        {
            var result = new CommandResult();
            if (funcs.IsNullOrEmpty()) return result;
            using (var tr = NewTransactionHelper())
            {
                if (!TryGetObjectId(handleValue, out var objectId))
                    return result.Cancel(ArgumentExceptionOfInvalidHandle._(handleValue.HandleAsLong));
                result = tr.RunFuncsOnObject(objectId, funcs);
                tr.Commit();
                return result;
            }
        }

        public CommandResult CreateInModelSpace<T>(out HandleValue handleValue,
            params Func<T, CommandResult>[] funcs)
            where T : Entity, new()
        {
            handleValue = null;
            using (var tr = NewTransactionHelper())
            {
                handleValue = tr.CreateInModelSpace<T>(CadDatabase.CurrentSpaceId);
                tr.Commit();
            }

            return RunFuncInTransaction(handleValue, funcs);
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

        public CommandResult ExecuteCommand()
        {
            throw new NotImplementedException();
        }
    }
}