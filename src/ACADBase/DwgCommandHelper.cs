using System;
using System.Collections.Generic;
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

        public List<Action<Database>> DatabaseActions { get; set; } = new List<Action<Database>>();

        protected Database DwgDatabase { get; set; }

        protected bool DefaultDrawing { get; private set; } = true;

        public IMessageProvider ActiveMsgProvider
        {
            get => _messageProvider;
            private set => _messageProvider = value ?? new MessageProviderOfMessageBox();
        }

        public void ExecuteDataBaseActions(params Action<Database>[] databaseActions)
        {
            if (databaseActions == null || databaseActions.Length == 0) return;
            DatabaseActions = databaseActions.ToList();
            ReadDwgAndExecute();
        }

        public void WriteMessage(string message)
        {
            ActiveMsgProvider.Show(message);
        }

        public void ShowError(Exception exception)
        {
            ActiveMsgProvider.Error(exception);
        }


        //TODO Can't verify if acedDisableDefaultARXExceptionHandler is working
        // EntryPoint may vary across autocad versions
        [DllImport("accore.dll", EntryPoint = "?acedDisableDefaultARXExceptionHandler@@YAX_N@Z")]
        public static extern void acedDisableDefaultARXExceptionHandler(int value);

        protected void ReadDwgAndExecute()
        {
            ExceptionInfo = null;
            acedDisableDefaultARXExceptionHandler(1);
            // Lock the document and execute the test actions.
            using (DwgDocument.LockDocument())
            using (DwgDatabase = new Database(DefaultDrawing, false))
            {
                Database oldDb = null!;
                if (!string.IsNullOrEmpty(DrawingFile))
                {
                    DwgDatabase.ReadDwgFile(DrawingFile, FileOpenMode.OpenForReadAndWriteNoShare, true, null);
                    oldDb = HostApplicationServices.WorkingDatabase;
                }
                else
                {
                    DwgDatabase = DwgDocument.Database;
                }

                HostApplicationServices.WorkingDatabase = DwgDatabase; // change to the current database.
                try
                {
                    RunCommandActions();
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                    ExceptionInfo = ExceptionDispatchInfo.Capture(e);
                }

                // Change the database back to the original.
                if (oldDb != null) HostApplicationServices.WorkingDatabase = oldDb;
                //TODO Throw exception here will cause fatal error and can not be catch by Nunit.
                if (ExceptionInfo != null) ActiveMsgProvider!.Error(ExceptionInfo.SourceException);
            }
        }

        /// <summary>
        ///     Override this method to run the command actions.
        /// </summary>
        protected virtual void RunCommandActions()
        {
            if (!DatabaseActions.Any()) return;
            DatabaseActions?.ToList().ForEach(action => action(DwgDatabase!));
        }
    }
}