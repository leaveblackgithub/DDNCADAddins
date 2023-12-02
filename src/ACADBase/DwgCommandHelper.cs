#nullable enable
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
        private readonly IMessageProvider? _messageProvider;
        protected Document DwgDocument;
        protected ExceptionDispatchInfo? ExceptionInfo;

        public DwgCommandHelper(string drawingFile = "", IMessageProvider? messageProvider = null)
        {
            DrawingFile = drawingFile;
            _messageProvider = messageProvider;
            if (messageProvider == null) _messageProvider = new MessageProviderOfMessageBox();
            DatabaseActions = new List<Action<Database>>();
            DwgDocument = Application.DocumentManager.MdiActiveDocument;
            if (string.IsNullOrEmpty(DrawingFile))
            {
                DefaultDrawing = true;
                // Should this be executing assembly path instead?
                //drawingFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestDrawing.dwg");
            }
            else
            {
                DefaultDrawing = false;
                if (!File.Exists(drawingFile))
                {
                    var argumentException = new ArgumentException($"Drawing file {drawingFile} does not exist.");
                    _logger.Error(argumentException);
                    throw argumentException;
                }
            }
        }

        protected string DrawingFile { get; }
        public List<Action<Database>> DatabaseActions { get; set; }
        protected Database? DwgDatabase { get; set; }
        protected bool DefaultDrawing { get; }
        
        public void ExecuteDataBaseActions(params Action<Database>[]? databaseActions)
        {
            if (databaseActions == null) return;
            DatabaseActions = databaseActions.ToList();
            ReadDwgAndExecute();
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
                if (ExceptionInfo != null) _messageProvider!.Error(ExceptionInfo.SourceException);
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