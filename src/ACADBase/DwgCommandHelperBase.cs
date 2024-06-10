using System;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.Misc;

namespace ACADBase
{
    public class DwgCommandHelperBase : DisposableClass

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

        public DwgCommandHelperBase()
        {
            InitiateEnvironment();
        }

        public DwgCommandHelperBase(string drawingFile = "")
        {
            InitiateEnvironment();
            DrawingFile = drawingFile;
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

        public static OperationResult<VoidValue> ExecuteCustomCommands<T>(string drawingFile,
            params Func<T, OperationResult<VoidValue>>[] funcs)
            where T : DwgCommandHelperBase, new()
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
                    resultDwgCmdHelper.Then(() => dwgCommandHelper.InitiateCmdDataBaseHelper());
                if (funcs.IsNullOrEmpty()) return resultVoid;
                foreach (var func in funcs) resultVoid = resultVoid.Then(() => func(dwgCommandHelper));
                return resultVoid;
            }
            catch (Exception e)
            {
                errMessage = e.Message;
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
        }

        protected override void DisposeManaged()
        {
        }
    }
}