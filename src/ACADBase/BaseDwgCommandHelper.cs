using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.Misc;

namespace ACADBase
{
    public class BaseDwgCommandHelper : DisposableClass
        //NO interface due to static invoke
    {
        // public virtual OperationResult<VoidValue> CustomExecute()
        // {
        //     throw new NotImplementedException();
        // }

        private bool _activeDbSwitched;
        private DocumentLock _documentLock;
        private string _drawingFile;
        private Database _oldDb;


        public IDatabaseHelper FldCmdDatabaseHelper;

        public BaseDwgCommandHelper()
        {
            InitiateFields();
        }

        public BaseDwgCommandHelper(string drawingFile = "")
        {
            InitiateFields();
            DrawingFile = drawingFile;
        }

        protected virtual IDatabaseHelper CommandDataBaseHelper
        {
            get
            {
                if (FldCmdDatabaseHelper == null) InitiateEnvironment();
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
        public delegate OperationResult<VoidValue> CustomCommandDelegate();
        public static OperationResult<VoidValue> ExecuteCustomCommands<T>(string drawingFile,
            params string[] methodNames)
            where T : BaseDwgCommandHelper, new()
        {
            string errMessage;
            var resultVoid = drawingFile.IsDefaultOrExistingDwg();
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
                var resultDwgCmdHelper = resultVoid.Then(() =>
                    ReflectionExtension.CreateInstance<T>(new object[] { drawingFile }));
                if (!resultDwgCmdHelper.IsSuccess)
                {
                    errMessage = resultDwgCmdHelper.ErrorMessage;
                    return OperationResult<VoidValue>.Failure(errMessage);
                }

                using var dwgCommandHelper = resultDwgCmdHelper.ReturnValue;
                resultVoid =
                    resultDwgCmdHelper.Then(() => dwgCommandHelper.InitiateEnvironment());
                resultVoid = resultVoid.Then(() => dwgCommandHelper.RunForEach(methodNames));
                return resultVoid;
            }
            catch (Exception e)
            {
                errMessage = e.Message;
                return OperationResult<VoidValue>.Failure(errMessage);
            }
        }

        public OperationResult<VoidValue> RunForEach(string[] methodNames)
        {
            OperationResult<VoidValue> result = OperationResult<VoidValue>.Success();
            if(methodNames.IsNullOrEmpty()) return result;
            foreach (var methodName in methodNames)
            {
                var invokeResult=
                this.MethodInvoke<OperationResult<VoidValue>>(methodName, new object[] { });
                if (!invokeResult.IsSuccess) return OperationResult<VoidValue>.Failure(invokeResult.ErrorMessage);
            }

            return result;
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

        protected virtual OperationResult<VoidValue> InitiateEnvironment()
        {
            var resultLock= DocumentManagerWrapper.LockActiveDocument();
            if(!resultLock.IsSuccess) return OperationResult<VoidValue>.Failure(resultLock.ErrorMessage);
            _documentLock = resultLock.ReturnValue;
            OperationResult<IDatabaseHelper> resultDataBaseHelper;
#if AcConsoleTest
            if (!HostApplicationServiceWrapper.IsTargetDrawingActive(DrawingFile).IsSuccess)
            {
                SaveWorkingDatabase();
                _activeDbSwitched = true;
                // return result;
            }

            resultDataBaseHelper =
                DatabaseHelper.NewDatabaseHelper<DatabaseHelperOfAcConsole>(DrawingFile);
#else
            resultDataBaseHelper =
                DatabaseHelper.NewDatabaseHelper<DatabaseHelperOfApplication>(DrawingFile);
#endif
            if (!resultDataBaseHelper.IsSuccess)
                return OperationResult<VoidValue>.Failure(resultDataBaseHelper.ErrorMessage);
            FldCmdDatabaseHelper = resultDataBaseHelper.ReturnValue;
            return OperationResult<VoidValue>.Success();
        }

        protected void InitiateFields()
        {
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
        }

        protected override void DisposeManaged()
        {
        }
    }
}