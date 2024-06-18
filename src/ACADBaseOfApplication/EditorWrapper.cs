using System;
using System.Data.SqlTypes;
using ACADBase;
using Autodesk.AutoCAD.EditorInput;
using CommonUtils.Misc;

namespace ACADBaseOfApplication
{
    public class EditorWrapper
    {

        private Editor _activeEditor;

        private EditorWrapper(Editor editor)
        {
            _activeEditor = editor;
        }

        public static OperationResult<EditorWrapper> GetActiveEditorWrapper()
        {
            var resultEditor = DocumentManagerWrapper.GetActiveEditor();
            return resultEditor.IsSuccess
                ? OperationResult<EditorWrapper>.Success(new EditorWrapper(resultEditor.ReturnValue))
                : OperationResult<EditorWrapper>.Failure(resultEditor.ErrorMessage);
        }
        public void WriteMessage(string message)
        {
            _activeEditor.WriteMessage(message);
        }
    }
}