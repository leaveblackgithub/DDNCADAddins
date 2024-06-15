using ACADAddins.Archive;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace ACADAddins.LibsOfDDNCrop
{
    internal class CopyBlock : CommandBaseOfDDNCrop
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