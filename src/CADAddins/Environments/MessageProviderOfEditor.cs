using System;
using ACADBase;
using Autodesk.AutoCAD.EditorInput;

namespace CADAddins.Environments
{
    public class MessageProviderOfEditor : IMessageProvider
    {
        private readonly Editor _editor;

        //use editor to construct this class
        public MessageProviderOfEditor(Editor editor)
        {
            _editor = editor;
        }

        public void Show(string message)
        {
            _editor.WriteMessage(message);
        }

        public void Error(Exception exception)
        {
            _editor.WriteMessage(exception.ToString());
        }
    }
}