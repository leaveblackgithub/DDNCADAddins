using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using CADAddins.Archive;
using CADAddins.Environments;

namespace CADAddins.LibsOfDDNCrop
{
    internal class CopyBlock : O_CommandBase2
    {
        internal override O_CommandTransBase InitCommandTransBase(Transaction acTrans)
        {
            return new OCommandTransBaseOfDuplicateBlockAs(acTrans);
        }

        [CommandMethod("CopyXC")]
        public override void Run()
        {
            base.Run();
        }
    }
}