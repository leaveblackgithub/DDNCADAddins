using Autodesk.AutoCAD.Runtime;
using DDNCADAddins;
using DDNCADAddins.Archive;
using DDNCADAddins.Environments;

[assembly: CommandClass(typeof(CleanupTextStyles))]

namespace DDNCADAddins
{
    public class CleanupTextStyles : O_CommandBase
    {
        private TextStyleHelper _curTextStyleHelper;

        public TextStyleHelper CurTextStyleHelper =>
            _curTextStyleHelper ?? (_curTextStyleHelper =
                new TextStyleHelper(AcCurDb.TextStyleTableId, CurDocHelper));

        [CommandMethod("CleanupTextStyles")]
        public override void RunCommand()
        {
            CurDocHelper.PurgeAll();
            //            HostApplicationServicesExtensions.CallUserBreak();
            CurTextStyleHelper.Cleanup();
            CurDocHelper.PurgeAll();
        }
    }
}