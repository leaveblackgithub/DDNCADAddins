using ACADAddins.Archive;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace ACADAddins.LibsOfDDNCrop
{
    public class HatchUtils : CommandBaseOfDDNCrop
    {
        internal override O_CommandTransBase InitCommandTransBase(Transaction acTrans)
        {
            return new OCommandTransBaseOfHatchUtils(acTrans);
        }

        [CommandMethod("RHB", CommandFlags.Modal)]
        public override void Run()
        {
            base.Run();
        }
    }
}