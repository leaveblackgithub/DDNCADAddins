using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using CADAddins.General;

namespace CADAddins.Environments
{
    public abstract class CommandBase 
    {
        internal CommandBase()
        {
        }

        internal DocHelper CurDocHelper => CadHelper.CurDocHelper;
        internal EditorHelper CurEditorHelper => CurDocHelper.CurEditorHelper;
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
            CadHelper.Quit();
        }

        internal abstract CommandTransBase InitCommandTransBase(Transaction acTrans);

        [CommandMethod("DDNCrop")]
        public virtual void Run()
        {
            using (Transaction acTrans = CurDocHelper.StartTransaction())
            {
                using (CommandTransBase cmd = InitCommandTransBase(acTrans))
                {
                    if (cmd.Run())
                    {
                        acTrans.Commit();
                        EndCommands(true);
                    }
                    else
                    {
                        acTrans.Abort();
                        EndCommands(false);
                    }
                }
            }
        }
    }
}