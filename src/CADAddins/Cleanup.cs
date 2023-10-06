using Autodesk.AutoCAD.Runtime;
using CADAddins;
using CADAddins.Archive;
using CADAddins.LibsOfCleanup;

[assembly: CommandClass(typeof(Cleanup))]

namespace CADAddins
{
    public class Cleanup : O_CommandBase
    {
        private LayerHelper _curLayerHelper;
        private LTypeHelper _curLTypeHelper;


        public LayerHelper CurLayerHelper =>
            _curLayerHelper ?? (_curLayerHelper =
                new LayerHelper(AcCurDb.LayerTableId, O_CurDocHelper, CurLTypeHelper));

        public LTypeHelper CurLTypeHelper =>
            _curLTypeHelper ?? (_curLTypeHelper =
                new LTypeHelper(AcCurDb.LinetypeTableId, O_CurDocHelper));

        [CommandMethod("Cleanup")]
        public override void RunCommand()
        {
            O_CurEditorHelper.WriteMessage($"\n开始清理文件[{O_CurDocHelper.Name}]...");
            O_CurDocHelper.StopHatchAssoc();
            CurLTypeHelper.Cleanup();
            CurLayerHelper.Cleanup();
            O_CurDocHelper.Audit();
            O_CurDocHelper.PurgeAll();
            CurLayerHelper.CleanupEnts();
            CurLayerHelper.CleanupBlocks();
            O_CurDocHelper.SetByLayer();
            O_CurDocHelper.PurgeAll();
            O_CadHelper.Quit();
        }
    }
}