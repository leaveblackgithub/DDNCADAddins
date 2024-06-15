using System;
using ACADBase;
using Autodesk.AutoCAD.EditorInput;

namespace ACADBaseOfApplication
{
    public class ActiveEditorWrapper
    {
        private static readonly Lazy<ActiveEditorWrapper> Instance =
            new Lazy<ActiveEditorWrapper>(() => new ActiveEditorWrapper());

        private ActiveEditorWrapper()
        {
        }

        public static ActiveEditorWrapper _ => Instance.Value;

        public Editor ActiveEditor => DocumentManagerWrapper.GetActiveEditor();

        public void WriteMessage(string message)
        {
            ActiveEditor.WriteMessage(message);
        }
    }
}