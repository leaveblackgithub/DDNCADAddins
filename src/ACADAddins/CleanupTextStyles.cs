using ACADAddins;
using ACADAddins.Archive;
using ACADAddins.LibsOfCleanupTextStyles;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(CleanupTextStyles))]

namespace ACADAddins
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