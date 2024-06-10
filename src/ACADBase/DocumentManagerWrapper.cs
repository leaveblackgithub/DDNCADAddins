using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.EditorInput;
using CommonUtils.CustomExceptions;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace ACADBase
{
    public static class DocumentManagerWrapper
    {
        public static Document GetActiveDocument()
        {
            var document = Application.DocumentManager.MdiActiveDocument;
            if (document == null) throw NullReferenceExceptionOfActiveDocument._();
            return document;
        }

        public static DocumentLock LockActiveDocument()
        {
            return GetActiveDocument().LockDocument();
        }

        public static Editor GetActiveEditor()
        {
            return GetActiveDocument().Editor;
        }
    }
}