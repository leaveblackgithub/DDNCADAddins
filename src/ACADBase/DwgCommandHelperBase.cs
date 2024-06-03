using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.Misc;
using System;
using System.IO;
using System.Runtime.InteropServices;
using CommonUtils.DwgLibs;
using CommonUtils.CustomExceptions;
using System.Reflection;

namespace ACADBase
{
    public class DwgCommandHelperBase : DisposableClass,IDwgCommandHelperInAcadBase, IDwgCommandHelper

    {
        private Database _oldDb;
        private bool _activeDbSwitched;
        private DocumentLock _documentLock;

        public static CommandResult ExecuteDwgCommandHelper<T>(string drawingFile = "", IMessageProvider messageProvider = null) 
            where T:DwgCommandHelperBase,new()
        {
            CommandResult result = new CommandResult();
            // T dwgCommandHelper = null;
            try
            {
                ConstructorInfo constructorInfo =
                    ReflectionExtension.GetConstructorInfo<T>(new Type[] { typeof(string), typeof(IMessageProvider) });
                using T dwgCommandHelper = (T)constructorInfo.Invoke(new object[] { drawingFile, messageProvider });
                result = dwgCommandHelper.Execute();
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
        

        //databasehelper是和dwgcommmandhelper一一对应的指针，而这些funcs应该是databasehelper的方法。为啥搞这么复杂
        public CommandResult ExecuteDatabaseFuncs(params Func<IDatabaseHelper, CommandResult>[] databaseFuncs)
        {
            var result = new CommandResult();
            if (databaseFuncs.IsNullOrEmpty()) return result;

            // using (_documentLock=AppActiveDocument.LockDocument())
            using (var db = CommandDataBaseHelper)
            {
                //exception and message has been handled in RunForEach
                result = databaseFuncs.RunForEach(db, ActiveMsgProvider);
                RestoreWorkingDatabase();
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
                _activeDbSwitched=false;
            }
        }
        //TODO Can't verify if acedDisableDefaultARXExceptionHandler is working
            // EntryPoint may vary across autocad versions
            [DllImport("accore.dll", EntryPoint = "?acedDisableDefaultARXExceptionHandler@@YAX_N@Z")]
        public static extern void acedDisableDefaultARXExceptionHandler(bool disable);

        protected IDatabaseHelper FldCmdDatabaseHelper;
        private string _drawingFile;
        private IMessageProvider _messageProvider;

        protected virtual IDatabaseHelper CommandDataBaseHelper
        {
            get
            {
                if (FldCmdDatabaseHelper == null)
                {
#if ApplicationTest
                    //TODO 无法解决Application里将新开图纸激活的问题，所以Application Command需要在当前图纸解决；
                    if (!HostApplicationServiceWrapper.IsTargetDrawingActive(DrawingFile))
                        throw ArgumentExceptionOfInvalidDwgFile._(DrawingFile);
                    FldCmdDatabaseHelper = new DatabaseHelperOfApplication();
#else
                    if (!HostApplicationServiceWrapper.IsTargetDrawingActive(DrawingFile))
                    {
                        SaveWorkingDatabase();
                        _activeDbSwitched = true;
                    }
                    FldCmdDatabaseHelper = new DatabaseHelperOfAcConsole(DrawingFile);

#endif
                }

                return FldCmdDatabaseHelper;
            }
            set
            {
                FldCmdDatabaseHelper = value;
            }
        }

        protected string DrawingFile
        {
            get => _drawingFile;
            set
            {
                _drawingFile = value;
                if (string.IsNullOrEmpty(_drawingFile)) return;
                if (!File.Exists(_drawingFile)) throw DwgFileNotFoundException._(_drawingFile);
            }
        }

        public IMessageProvider ActiveMsgProvider
        {
            get => _messageProvider;
            protected set => _messageProvider = value ?? new MessageProviderOfMessageBox();
        }

        protected  void InitiateEnvironment()
        {
            _documentLock = DocumentManagerExtension.LockActiveDocument();
            _oldDb = null;
            _activeDbSwitched = false;
            //如果改为TRUE，未处理exception会导致cad崩溃???
            acedDisableDefaultARXExceptionHandler(true);
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

        public void WriteMessage(string message)
        {
            ActiveMsgProvider.Show(message);
        }

        public void ShowError(Exception exception)
        {
            ActiveMsgProvider.Error(exception);
        }

        public virtual CommandResult Execute()
        {
            throw new NotImplementedException();
        }
    }
}