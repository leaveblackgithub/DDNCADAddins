using Autodesk.AutoCAD.Runtime;
using CADAddins;
using CADAddins.Archive;
using CADAddins.Environments;
using CADAddins.LibsOfCleanupTextStyles;

[assembly: CommandClass(typeof(CleanupTextStyles))]

namespace CADAddins
{
    public class CleanupTextStyles : O_CommandBase
    {
        private TextStyleHelper _curTextStyleHelper;

        public TextStyleHelper CurTextStyleHelper =>
            _curTextStyleHelper ?? (_curTextStyleHelper =
                new TextStyleHelper(AcCurDb.TextStyleTableId, O_CurDocHelper));

        [CommandMethod("CleanupTextStyles")]
        public override void RunCommand()
        {
            O_CurDocHelper.PurgeAll();
            //            HostApplicationServicesExtensions.CallUserBreak();
            CurTextStyleHelper.Cleanup();
            O_CurDocHelper.PurgeAll();
        }
    }
}