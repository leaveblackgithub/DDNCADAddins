using Autodesk.AutoCAD.Runtime;
using PreviousDevelopmentToRefactor;
using PreviousDevelopmentToRefactor.Archive;
using PreviousDevelopmentToRefactor.Environments;

[assembly: CommandClass(typeof(CleanupTextStyles))]

namespace PreviousDevelopmentToRefactor
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