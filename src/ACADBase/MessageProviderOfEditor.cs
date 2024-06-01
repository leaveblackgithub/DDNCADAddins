using System;
using Autodesk.AutoCAD.EditorInput;
using CommonUtils.Misc;

namespace ACADBase
{
    public class MessageProviderOfEditor : MessageProviderBase
    {
        public MessageProviderOfEditor(Editor editor)
        {
            ActiveEditor=editor;
        }

        public Editor ActiveEditor { get;}

        public override void Show(string message)
        {
            ActiveEditor.WriteMessage(message);
        }
    }
}