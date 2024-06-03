using Autodesk.AutoCAD.ApplicationServices;
using CommonUtils.CustomExceptions;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace ACADBase
{
    public static class DocumentManagerExtension
    {
        public static Document GetActiveDocument()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            if (document == null) throw NullReferenceExceptionOfActiveDocument._();
            return document;
        }

        public static DocumentLock LockActiveDocument()
        {
            return GetActiveDocument().LockDocument();
        }
    }
}