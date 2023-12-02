using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using CADAddins.Archive;
using CADAddins.Environments;

namespace CADAddins.LibsOfDDNCrop
{
    public class HatchUtils : CommandBase
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