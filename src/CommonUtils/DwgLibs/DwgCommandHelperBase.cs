using System;
using System.IO;
using CommonUtils.CustomExceptions;
using CommonUtils.Misc;

namespace CommonUtils.DwgLibs
{
    public abstract class DwgCommandHelperBase : IDwgCommandHelper
    {
        private string _drawingFile;
        private IMessageProvider _messageProvider;

        public DwgCommandHelperBase(string drawingFile = "", IMessageProvider messageProvider = null)
        {
            DrawingFile = drawingFile;
            ActiveMsgProvider = messageProvider;
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

        public abstract CommandResult ExecuteCommandInDbHelper();
    }
}