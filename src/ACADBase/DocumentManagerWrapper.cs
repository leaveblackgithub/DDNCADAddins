using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.EditorInput;
using CommonUtils.Misc;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace ACADBase
{
    public static class DocumentManagerWrapper
    {
        public static OperationResult<Document> GetActiveDocument()
        {
            var document = Application.DocumentManager.MdiActiveDocument;
            return document == null
                ? OperationResult<Document>.Failure(ExceptionMessage.NoActiveDocument())
                : OperationResult<Document>.Success(document);
        }

        public static OperationResult<DocumentLock> LockActiveDocument()
        {
            var resultDocument = GetActiveDocument();
            return resultDocument.IsSuccess
                ? OperationResult<DocumentLock>.Success(resultDocument.ReturnValue.LockDocument())
                : OperationResult<DocumentLock>.Failure(resultDocument.ErrorMessage);
        }

        public static OperationResult<Editor> GetActiveEditor()
        {
            var resultDocument = GetActiveDocument();
            return resultDocument.IsSuccess
                ? OperationResult<Editor>.Success(resultDocument.ReturnValue.Editor)
                : OperationResult<Editor>.Failure(resultDocument.ErrorMessage);
        }
    }
}