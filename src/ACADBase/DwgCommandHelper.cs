using System;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using NLog;

namespace ACADBase
{
    /// <summary>
    ///     Base class for CadCommand.
    /// </summary>
    public class DwgCommandHelper : IDwgCommandHelper
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private string _drawingFile;
        private IMessageProvider _messageProvider;
        protected Document DwgDocument;
        protected ExceptionDispatchInfo ExceptionInfo;

        public DwgCommandHelper(string drawingFile = "", IMessageProvider messageProvider = null)
        {
            DrawingFile = drawingFile;
            ActiveMsgProvider = messageProvider;
            DwgDocument = Application.DocumentManager.MdiActiveDocument;
        }

        protected string DrawingFile
        {
            get => _drawingFile;
            set
            {
                _drawingFile = value;
                if (string.IsNullOrEmpty(_drawingFile)) return;
                DefaultDrawing = false;
                if (File.Exists(_drawingFile)) return;
                var argumentException = new ArgumentException($"Drawing file {_drawingFile} does not exist.");
                _logger.Error(argumentException);
                throw argumentException;
            }
        }

        protected bool DefaultDrawing { get; private set; } = true;

        public IMessageProvider ActiveMsgProvider
        {
            get => _messageProvider;
            private set => _messageProvider = value ?? new MessageProviderOfMessageBox();
        }

        public void WriteMessage(string message)
        {
            ActiveMsgProvider.Show(message);
        }

        public void ShowError(Exception exception)
        {
            ActiveMsgProvider.Error(exception);
        }

        public void ExecuteDataBaseActions(params Action<Database>[] databaseActions)
        {
            if (databaseActions == null || databaseActions.Length == 0) return;
            ExceptionInfo = null;
            acedDisableDefaultARXExceptionHandler(1);
            // Lock the document and execute the test actions.
            using (DwgDocument.LockDocument())
            using (var oldDb = GetActiveDatabaseBeforeCommand())
            using (var db = GetDwgDatabase())
            {
                try
                {
                    databaseActions.ToList().ForEach(action => action(db));
                }

                catch (Exception e)
                {
                    _logger.Error(e);
                    ExceptionInfo = ExceptionDispatchInfo.Capture(e);
                }

                if (!IsNewDrawingOrExisting()) HostApplicationServices.WorkingDatabase = oldDb;
            }

            //TODO Throw exception here will cause fatal error and can not be catch by Nunit.
            if (ExceptionInfo != null) ActiveMsgProvider.Error(ExceptionInfo.SourceException);
        }

        protected Database GetActiveDatabaseBeforeCommand()
        {
            return IsNewDrawingOrExisting() ? null : HostApplicationServices.WorkingDatabase;
        }

        protected Database GetDwgDatabase()
        {
            Database dwgDatabase;
            dwgDatabase = new Database(DefaultDrawing, false);

            if (IsNewDrawingOrExisting())
                dwgDatabase = DwgDocument.Database;
            else
                dwgDatabase.ReadDwgFile(DrawingFile, FileOpenMode.OpenForReadAndWriteNoShare, true, null);
            return dwgDatabase;
        }

        private bool IsNewDrawingOrExisting()
        {
            return string.IsNullOrEmpty(DrawingFile);
        }


        //TODO Can't verify if acedDisableDefaultARXExceptionHandler is working
        // EntryPoint may vary across autocad versions
        [DllImport("accore.dll", EntryPoint = "?acedDisableDefaultARXExceptionHandler@@YAX_N@Z")]
        public static extern void acedDisableDefaultARXExceptionHandler(int value);
    }
}