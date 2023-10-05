using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using CADAddins.Environments;

[assembly: CommandClass(typeof(CADAddins.HatchUtils))]

namespace CADAddins
{
    public class HatchUtils : CommandBase
    {
        internal override CommandTransBase InitCommandTransBase(Transaction acTrans)
        {
            return new CommandTransBaseOfHatchUtils(acTrans);
        }

        [CommandMethod("RHB", CommandFlags.Modal)]
        public override void Run()
        {
            base.Run();
        }
    }

    public class CommandTransBaseOfHatchUtils : CommandTransBase
    {
        public CommandTransBaseOfHatchUtils(Transaction acTrans) : base(acTrans)
        {
        }

        public override bool Run()
        {
            var hatchId = CurEditorHelper.GetEntity("\nSelect Hatch: ", "\nNot a Hatch", typeof(Hatch));
            if (hatchId == ObjectId.Null) return false;
            var hatch = GetObjectForRead(hatchId) as Hatch;
            if (hatch == null) return false;
            CurEditorHelper.WriteMessage(hatch.GetHatchLoopTypes());
            hatch.GenerateHatchBoundaries(this);

            return true;
        }
    }
}