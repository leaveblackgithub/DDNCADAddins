using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils;

namespace CADAddins.Archive
{
    public abstract class O_CommandBase : DisposableClass
    {

        public static MyMessageFilter Filter;

        protected O_CommandBase()
        {
            
        }

        public O_DocHelper O_CurDocHelper => O_CadHelper.CurDocHelper;
        public Database AcCurDb => O_CurDocHelper.AcCurDb;
        public O_EditorHelper O_CurEditorHelper => O_CurDocHelper.CurEditorHelper;

        public abstract void RunCommand();

        protected override void DisposeUnManaged()
        {
            O_CadHelper.Quit();
        }

        protected override void DisposeManaged()
        {
        }

        protected void EndCommands(bool EndOrCancel)
        {
            if (!EndOrCancel)
                O_CurEditorHelper.WriteMessage($"\nCommand [{GetType().Name}] Cancelled.");
            else
                O_CurEditorHelper.WriteMessage($"\nCommand [{GetType().Name}] Succeeded.");
            Dispose();
        }

        public static bool CancelLoop(O_EditorHelper ed)
        {
            // Check for user input events
            System.Windows.Forms.Application.DoEvents();
            // Check whether the filter has set the flag
            if (Cleanup.Filter.bCanceled)
            {
                ed.WriteMessage("command cancelled.");
                return true;
            }

            return false;
        }

        public static void RemoveMessageFilter()
        {


            // We're done - remove the message filter
            if(Filter == null) return;
            System.Windows.Forms.Application.RemoveMessageFilter(Filter);
            Filter = null;
        }

        public static void AddMessageFilter()
        {
            // Create and add our message filter
            Filter = new MyMessageFilter();
            System.Windows.Forms.Application.AddMessageFilter(Filter);
        }
    }
}