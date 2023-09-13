using Autodesk.AutoCAD.DatabaseServices;
using CADAddins.General;

namespace CADAddins.Archive
{
    public abstract class O_CommandBase:DisposableClass
    {
        internal O_CommandBase()
        {
        }

        public abstract void RunCommand();
        internal O_DocHelper CurDocHelper => O_CadHelper.CurDocHelper;
        internal Database AcCurDb => CurDocHelper.AcCurDb;
        internal O_EditorHelper CurEditorHelper => CurDocHelper.CurEditorHelper;

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
            {
                CurEditorHelper.WriteMessage($"\nCommand [{this.GetType().Name}] Cancelled.");
            }
            else
            {
                CurEditorHelper.WriteMessage($"\nCommand [{this.GetType().Name}] Succeeded.");
            }
            Dispose();
        }
    }
}