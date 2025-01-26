using System.Collections.Generic;
using System.Reflection;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using CADAddins.General;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace CADAddins.Environments
{
    public class DocHelper : DisposableClass
    {
        private EditorHelper _curEditorHelper;

        public DocHelper(Document acDoc)
        {
            AcDoc = acDoc;
        }

        public Document AcDoc { get; }

        public Database AcCurDb => HostApplicationServices.WorkingDatabase;

        public ObjectId CurSpaceId => AcCurDb.CurrentSpaceId;
        public ObjectId BlockTableId => AcCurDb.BlockTableId;
        public EditorHelper CurEditorHelper =>
            _curEditorHelper ?? (_curEditorHelper =
                new EditorHelper(AcDoc.Editor));

        public string Name => AcDoc.Name;

        public Transaction StartTransaction()
        {
            return AcDoc.TransactionManager.StartTransaction();
        }


        public void Audit()
        {
            using (var trans = StartTransaction())
            {
                AcCurDb.Audit(true, false);
            }
        }

        protected override void DisposeUnManaged()
        {
        }

        protected override void DisposeManaged()
        {
        }
    }
}