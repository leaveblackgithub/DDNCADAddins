using Autodesk.AutoCAD.DatabaseServices;
using CADAddins.Archive;

namespace CADAddins.LibsOfDDNCrop
{
    public abstract class CommandBaseOfDDNCrop
    {
        internal CommandBaseOfDDNCrop()
        {
        }

        internal O_DocHelper2 CurODocHelper2 => O_CadHelper2.CurODocHelper2;
        internal O_EditorHelper2 CurOEditorHelper2 => CurODocHelper2.CurOEditorHelper2;

        internal void EndCommands(bool EndOrCancel)
        {
            if (!EndOrCancel)
                CurOEditorHelper2.WriteMessage($"\nCommand [{GetType().Name}] Cancelled.");
            else
                CurOEditorHelper2.WriteMessage($"\nCommand [{GetType().Name}] Succeeded.");
            O_CadHelper2.Quit();
        }

        internal abstract O_CommandTransBase InitCommandTransBase(Transaction acTrans);

        public virtual void Run()
        {
            using (var acTrans = CurODocHelper2.StartTransaction())
            {
                using (var cmd = InitCommandTransBase(acTrans))
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