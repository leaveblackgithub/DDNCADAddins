using Autodesk.AutoCAD.ApplicationServices;
using CommonUtils.Misc;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace ACADBase
{
    public static class DocumentManagerWrapper
    {
        public static FuncResult GetActiveDocument(out Document document)
        {
            FuncResult result=new FuncResult();
            document = Application.DocumentManager.MdiActiveDocument;
            if (document == null) return result.Cancel(ExceptionMessage.NullActiveDocument());
            return result;
        }

        public static FuncResult LockActiveDocument(out DocumentLock documentLock)
        {
            FuncResult result = GetActiveDocument(out var document);
            documentLock = document.LockDocument();
            return result;
        }
    }
}