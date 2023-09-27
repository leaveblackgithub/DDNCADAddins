using Autodesk.AutoCAD.ApplicationServices;
using Domain.Shared;

namespace ACADWrappers.Shared
{
    public class DocumentWrapper:IDocumentWrapper
    {
        public Document DwgDocument { get; }
        public IDatabaseWrapper CurDbWrapper { get; }

        public DocumentWrapper(Document dwgDocument)
        {
            DwgDocument = dwgDocument;
            CurDbWrapper = new DatabaseWrapper(DwgDocument.Database);
        }
    }
}