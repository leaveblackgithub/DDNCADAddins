using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils;
using CommonUtils.Misc;

namespace CADAddins.Archive
{
    public class O_DocHelper2 : DisposableClass
    {
        private O_EditorHelper2 _curOEditorHelper2;

        public O_DocHelper2(Document acDoc)
        {
            AcDoc = acDoc;
        }

        public Document AcDoc { get; }

        public Database AcCurDb => HostApplicationServices.WorkingDatabase;

        public ObjectId CurSpaceId => AcCurDb.CurrentSpaceId;
        public ObjectId BlockTableId => AcCurDb.BlockTableId;

        public O_EditorHelper2 CurOEditorHelper2 =>
            _curOEditorHelper2 ?? (_curOEditorHelper2 =
                new O_EditorHelper2(AcDoc.Editor));

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