using ACADTests.IntegrateTests;
using Autodesk.AutoCAD.Runtime;
using CADAddins.Archive;
using CADAddins.LibsOfCleanup;

[assembly: CommandClass(typeof(CleanupCheck))]

namespace ACADTests.IntegrateTests
{
    public class CleanupCheck : O_CommandBase
    {
        private LayerHelper _curLayerHelper;
        private LTypeHelper _curLTypeHelper;


        public LayerHelper CurLayerHelper =>
            _curLayerHelper ?? (_curLayerHelper =
                new LayerHelper(AcCurDb.LayerTableId, O_CurDocHelper, CurLTypeHelper));

        public LTypeHelper CurLTypeHelper =>
            _curLTypeHelper ?? (_curLTypeHelper =
                new LTypeHelper(AcCurDb.LinetypeTableId, O_CurDocHelper));

        [CommandMethod("CleanupCheck")]
        public override void RunCommand()
        {
            O_CurEditorHelper.WriteMessage($"\nCleanup Result Check [{O_CurDocHelper.Name}]...");
            CurLTypeHelper.Check();
            O_CadHelper.Quit();
        }
    }
}