using System;
using System.IO;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.CustomExceptions;
using CommonUtils.DwgLibs;
using CommonUtils.Misc;
using CommonUtils.StringLibs;

namespace ACADBase
{
    public class DwgCommandHelperBase : DisposableClass, IDwgCommandHelper

    {
        private bool _activeDbSwitched;
        private DocumentLock _documentLock;
        private string _drawingFile;
        private IMessageProvider _messageProvider;
        private Database _oldDb;

        public IDatabaseHelper FldCmdDatabaseHelper;

        public DwgCommandHelperBase()
        {
            InitiateEnvironment();
        }

        public DwgCommandHelperBase(string drawingFile = "", IMessageProvider messageProvider = null)
        {
            InitiateEnvironment();
            DrawingFile = drawingFile;
            ActiveMsgProvider = messageProvider;
        }

        protected virtual IDatabaseHelper CommandDataBaseHelper
        {
            get
            {
                if (FldCmdDatabaseHelper == null) InitiateCmdDataBaseHelper();
                return FldCmdDatabaseHelper;
            }
            set => FldCmdDatabaseHelper = value;
        }

        protected string DrawingFile
        {
            get => _drawingFile;
            set
            {
                _drawingFile = value;
                if (string.IsNullOrEmpty(_drawingFile)) return;
            }
        }

        public IMessageProvider ActiveMsgProvider
        {
            get => _messageProvider;
            protected set => _messageProvider = value ?? new MessageProviderOfMessageBox();
        }

        

        public void WriteMessage(string message)
        {
            ActiveMsgProvider.Show(message);
        }

        public void ShowError(Exception exception)
        {
            ActiveMsgProvider.Error(exception);
        }

        public virtual OperationResult<VoidValue> CustomExecute()
        {
            throw new NotImplementedException();
        }

        public static OperationResult<VoidValue> ExecuteCustomCommands<T>(string drawingFile ,
            IMessageProvider messageProvider)
            where T : DwgCommandHelperBase, new()
        {
            string errMessage;
            OperationResult<VoidValue> resultVoid = drawingFile.IsDefaultOrExistingDwg();
            // var result = new FuncResult();
            // if (drawingFile.IsNotDefaultOrExistingDwg().IsSuccess)
            //     return result.Cancel(DwgFileNotFoundException.CustomeMessage(drawingFile));
#if ApplicationTest
            //TODO 无法解决Application里将新开图纸激活的问题，所以Application Command需要在当前图纸解决；
            // if (!HostApplicationServiceWrapper.IsTargetDrawingActive(drawingFile).IsSuccess)
            //     return result.Cancel(ArgumentExceptionOfInvalidDwgFile.CustomeMessage(drawingFile));
            resultVoid = resultVoid.Then(() => HostApplicationServiceWrapper.IsTargetDrawingActive(drawingFile));

#endif
            // T dwgCommandHelper = null;
            try
            {
                OperationResult<T> resultDwgCmdHelper= resultVoid.Then<VoidValue, T>(() =>
                    ReflectionExtension.CreateInstance<T>(new object[] { drawingFile, messageProvider }));
                if (!resultDwgCmdHelper.IsSuccess)
                {
                    errMessage = resultDwgCmdHelper.ErrorMessage;
                    messageProvider.Show(errMessage);
                    return OperationResult<VoidValue>.Failure(errMessage);
                }

                using T dwgCommandHelper = resultDwgCmdHelper.ReturnValue;
                dwgCommandHelper.InitiateEnvironment();
                resultVoid =
                    resultDwgCmdHelper.Then<T, VoidValue>(() => dwgCommandHelper.InitiateCmdDataBaseHelper());
                resultVoid = resultVoid.Then(() => dwgCommandHelper.CustomExecute());
                if (!resultVoid.IsSuccess)
                {
                    messageProvider.Show(resultVoid.ErrorMessage);
                }
                return resultVoid;
            }
            catch (Exception e)
            {
                errMessage = e.Message;
                messageProvider.Show(errMessage);
                return OperationResult<VoidValue>.Failure(errMessage);
            }
        }

        private void SaveWorkingDatabase()
        {
            _oldDb = HostApplicationServiceWrapper.GetWorkingDatabase();
        }

        private void RestoreWorkingDatabase()
        {
            if (_activeDbSwitched)
            {
                HostApplicationServiceWrapper.SetWorkingDatabase(_oldDb);
                _oldDb = null;
                _activeDbSwitched = false;
            }
        }

        //TODO Can't verify if acedDisableDefaultARXExceptionHandler is working
        // EntryPoint may vary across autocad versions
        [DllImport("accore.dll", EntryPoint = "?acedDisableDefaultARXExceptionHandler@@YAX_N@Z")]
        public static extern void acedDisableDefaultARXExceptionHandler(bool disable);

        private OperationResult<VoidValue> InitiateCmdDataBaseHelper()
        {
            OperationResult<IDatabaseHelper> resultDataBaseHelper;
#if AcConsoleTest
            if (!HostApplicationServiceWrapper.IsTargetDrawingActive(DrawingFile).IsSuccess)
            {
                SaveWorkingDatabase();
                _activeDbSwitched = true;
                // return result;
            }

            resultDataBaseHelper= DatabaseHelper.NewDatabaseHelper<DatabaseHelperOfAcConsole>(DrawingFile, ActiveMsgProvider);
#else
            resultDataBaseHelper = DatabaseHelper.NewDatabaseHelper<DatabaseHelperOfApplication>(DrawingFile, ActiveMsgProvider);
#endif
            if (!resultDataBaseHelper.IsSuccess) return OperationResult<VoidValue>.Failure(resultDataBaseHelper.ErrorMessage);
            FldCmdDatabaseHelper = resultDataBaseHelper.ReturnValue;
            return OperationResult<VoidValue>.Success();
        }

        protected void InitiateEnvironment()
        {
            _documentLock = DocumentManagerWrapper.LockActiveDocument();
            _oldDb = null;
            _activeDbSwitched = false;
            //如果改为TRUE，未处理exception会导致cad崩溃???
            acedDisableDefaultARXExceptionHandler(false);
        }

        protected override void DisposeUnManaged()
        {
            RestoreWorkingDatabase();
            CommandDataBaseHelper?.Dispose();
            CommandDataBaseHelper = null;
            _documentLock?.Dispose();
            _documentLock = null;
            ActiveMsgProvider = null;
        }

        protected override void DisposeManaged()
        {
        }
    }
}