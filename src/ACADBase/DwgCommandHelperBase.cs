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

        public virtual CommandResult CustomExecute()
        {
            throw new NotImplementedException();
        }

        public static CommandResult ExecuteCustomCommands<T>(string drawingFile = "",
            IMessageProvider messageProvider = null)
            where T : DwgCommandHelperBase, new()
        {
            var result = new CommandResult();
            if (drawingFile != "" && (drawingFile.SubStringRight(4) != ".dwg" || !File.Exists(drawingFile)))
                return result.Cancel(DwgFileNotFoundException.CustomeMessage(drawingFile));
#if ApplicationTest
            //TODO 无法解决Application里将新开图纸激活的问题，所以Application Command需要在当前图纸解决；
            if (!HostApplicationServiceWrapper.IsTargetDrawingActive(drawingFile))
                return result.Cancel(ArgumentExceptionOfInvalidDwgFile.CustomeMessage(drawingFile));

#endif
            // T dwgCommandHelper = null;
            try
            {
                result = ReflectionExtension.GetConstructorInfo<T>
                    (new[] { typeof(string), typeof(IMessageProvider) }, out var constructorInfo);
                if (result.IsCancel) return result;
                using var dwgCommandHelper = (T)constructorInfo.Invoke(new object[] { drawingFile, messageProvider });
                result= dwgCommandHelper.InitiateCmdDataBaseHelper();
                if (result.IsCancel) return result;
                result = dwgCommandHelper.CustomExecute();
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

        private CommandResult InitiateCmdDataBaseHelper()
        {
            var result = new CommandResult();
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