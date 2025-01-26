using Autodesk.AutoCAD.Runtime;
using CADAddins;
using CADAddins.Archive;
using CADAddins.Environments;

[assembly: CommandClass(typeof(CleanupTextStyles))]

namespace CADAddins
{
    public class CleanupTextStyles : O_CommandBase
    {
        private TextStyleHelper _curTextStyleHelper;

        [CommandMethod("CleanupTextStyles")]
        public override void RunCommand()
        {
            CurDocHelper.PurgeAll();
            //            HostApplicationServicesExtensions.CallUserBreak();
            CurTextStyleHelper.Cleanup();
            CurDocHelper.PurgeAll();
        }
        public TextStyleHelper CurTextStyleHelper =>
            _curTextStyleHelper ?? (_curTextStyleHelper =
                new TextStyleHelper(AcCurDb.TextStyleTableId, CurDocHelper));
    }
}