using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.CustomExceptions;
using CommonUtils.Misc;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ACADBase
{
    public class DwgCommandHelperBase : IDwgCommandHelper
    {
        private string _drawingFile;
        private IMessageProvider _messageProvider;
        protected Document DwgDocument;

        public DwgCommandHelperBase(string drawingFile = "", IMessageProvider messageProvider = null)
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
                if (!File.Exists(_drawingFile)) throw DwgFileNotFoundException._(_drawingFile);
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

        public CommandResult ExecuteDatabaseFuncs(params Func<IDatabaseHelper, CommandResult>[] databaseFuncs)
        {
            var result = new CommandResult();
            if (databaseFuncs.IsNullOrEmpty()) return result;

            acedDisableDefaultARXExceptionHandler(0);
            // Lock the document and execute the test actions.

            var oldDb = GetActiveDatabaseBeforeCommand(); //WorkingDatabase can not be disposed.
            using (DwgDocument.LockDocument())
            using (var db = GetDwgDatabaseHelper())
            {
                try
                {
                    result = databaseFuncs.RunForEach(db);
                }

                catch (Exception e)
                {
                    result.Cancel(e);
                }

                if (!IsNewDrawingOrExisting()) HostApplicationServices.WorkingDatabase = oldDb;
            }

            //TODO Throw exception here will cause fatal error and can not be catch by Nunit.
            var resultExceptionInfo = result.ExceptionInfo;
            if (resultExceptionInfo != null) ActiveMsgProvider.Error(resultExceptionInfo.SourceException);
            return result;
        }


        protected bool IsNewDrawingOrExisting()
        {
            return string.IsNullOrEmpty(DrawingFile);
        }


        //TODO Can't verify if acedDisableDefaultARXExceptionHandler is working
        // EntryPoint may vary across autocad versions
        [DllImport("accore.dll", EntryPoint = "?acedDisableDefaultARXExceptionHandler@@YAX_N@Z")]
        public static extern void acedDisableDefaultARXExceptionHandler(int value);


        protected virtual Database GetActiveDatabaseBeforeCommand()
        {
            throw new NotImplementedException();
        }

        protected virtual IDatabaseHelper GetDwgDatabaseHelper()
        {
            throw new NotImplementedException();
        }
    }
}