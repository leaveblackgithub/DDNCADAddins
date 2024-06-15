using ACADAddins.Archive;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace ACADAddins.LibsOfDDNCrop
{
    public class ReadHatchLoopType : CommandBaseOfDDNCrop
    {
        [CommandMethod("RLT")]
        public override void Run()
        {
            base.Run();
        }

        internal override O_CommandTransBase InitCommandTransBase(Transaction acTrans)
        {
            return new OCommandTransBaseOfReadHatchLoopType(acTrans);
        }
    }
}