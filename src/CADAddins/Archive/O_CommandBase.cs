using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils;

namespace CADAddins.Archive
{
    public abstract class O_CommandBase : DisposableClass
    {
        internal O_CommandBase()
        {
        }

        internal O_DocHelper O_CurDocHelper => O_CadHelper.CurDocHelper;
        internal Database AcCurDb => O_CurDocHelper.AcCurDb;
        internal O_EditorHelper O_CurEditorHelper => O_CurDocHelper.CurEditorHelper;

        public abstract void RunCommand();

        protected override void DisposeUnManaged()
        {
            O_CadHelper.Quit();
        }

        protected override void DisposeManaged()
        {
        }

        internal void EndCommands(bool EndOrCancel)
        {
            if (!EndOrCancel)
                O_CurEditorHelper.WriteMessage($"\nCommand [{GetType().Name}] Cancelled.");
            else
                O_CurEditorHelper.WriteMessage($"\nCommand [{GetType().Name}] Succeeded.");
            Dispose();
        }
    }
}