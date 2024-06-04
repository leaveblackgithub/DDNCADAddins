using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.DwgLibs;
using CommonUtils.Misc;
using CommonUtils.StringLibs;

namespace ACADBase
{
    public abstract class DwgCommandHelperBase : DisposableClass, IDwgCommandHelper

    {
        private bool _activeDbSwitched;
        private DocumentLock _documentLock;
        private string _drawingFile;
        private IMessageProvider _messageProvider;
        private Database _oldDb;

        public IDatabaseHelper FldCmdDatabaseHelper;

        public DwgCommandHelperBase()
        {
        }

        public DwgCommandHelperBase(string drawingFile = "", IMessageProvider messageProvider = null)
        {
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

        public abstract FuncResult ExecuteMain();

        public FuncResult ExecuteFunc(params Func<FuncResult>[] funcs)
        {
            return funcs.RunForEach();
        }

        public static FuncResult _<T>(string drawingFile = "",
            IMessageProvider messageProvider = null)
            where T : DwgCommandHelperBase, new()
        {
            var result = new FuncResult();
            
            if (drawingFile != "" && (drawingFile.SubStringRight(4) != ".dwg" || !File.Exists(drawingFile)))
                return result.Cancel(ExceptionMessage.DwgFileNotFound(drawingFile));
#if ApplicationTest
            //TODO 无法解决Application里将新开图纸激活的问题，所以Application Command需要在当前图纸解决；
            if (!HostApplicationServiceWrapper.IsTargetDrawingActive(drawingFile))
                return result.Cancel(ArgumentExceptionOfInvalidDwgFile.CustomeMessage(drawingFile));

#endif
            // T dwgCommandHelper = null;
            try
            {
                T dwgCommandHelper = null;
                IList<Func<FuncResult>> funcs = new List<Func<FuncResult>>()
                {
                    () => ReflectionExtension.CreateInstance<T>(new object[]{drawingFile,messageProvider},out dwgCommandHelper),
                    dwgCommandHelper.InitiateEnvironment,
                    dwgCommandHelper.InitiateCmdDataBaseHelper,
                    dwgCommandHelper.ExecuteMain
                };
                if (result.IsCancel) return result;
                result = dwgCommandHelper.ExecuteMain();
            }
            catch (Exception e)
            {
                result.Cancel(e);
            }

            // finally
            // {
            //     dwgCommandHelper?.Dispose();
            //     dwgCommandHelper = null;
            //
            // }
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

        private FuncResult InitiateCmdDataBaseHelper()
        {
            var result = new FuncResult();
#if AcConsoleTest
            if (!HostApplicationServiceWrapper.IsTargetDrawingActive(DrawingFile))
            {
                SaveWorkingDatabase();
                _activeDbSwitched = true;
                // return result;
            }

            result =
                DatabaseHelper.NewDatabaseHelper<DatabaseHelperOfAcConsole>(DrawingFile, null,
                    out var commandDataHelper);
#else
            result = DatabaseHelper.NewDatabaseHelper<DatabaseHelperOfApplication>(DrawingFile, null,
                out var commandDataHelper);

#endif
            FldCmdDatabaseHelper = result.IsCancel ? null : commandDataHelper;
            return result;
        }

        protected FuncResult InitiateEnvironment()
        {
            var result = DocumentManagerWrapper.LockActiveDocument(out _documentLock);
            if(result.IsCancel)return result;
            _oldDb = null;
            _activeDbSwitched = false;
            //如果改为TRUE，未处理exception会导致cad崩溃???
            acedDisableDefaultARXExceptionHandler(false);
            return new FuncResult();
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