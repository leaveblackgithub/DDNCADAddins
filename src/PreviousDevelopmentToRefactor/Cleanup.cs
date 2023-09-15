using Autodesk.AutoCAD.Runtime;
using PreviousDevelopmentToRefactor;
using PreviousDevelopmentToRefactor.Archive;
using PreviousDevelopmentToRefactor.LibsOfCleanup;

[assembly: CommandClass(typeof(Cleanup))]

namespace PreviousDevelopmentToRefactor
{
    public class Cleanup : O_CommandBase
    {
        private LayerHelper _curLayerHelper;
        private LTypeHelper _curLTypeHelper;


        public LayerHelper CurLayerHelper =>
            _curLayerHelper ?? (_curLayerHelper =
                new LayerHelper(AcCurDb.LayerTableId, CurDocHelper, CurLTypeHelper));

        public LTypeHelper CurLTypeHelper =>
            _curLTypeHelper ?? (_curLTypeHelper =
                new LTypeHelper(AcCurDb.LinetypeTableId, CurDocHelper));

        [CommandMethod("Cleanup")]
        public override void RunCommand()
        {
            CurEditorHelper.WriteMessage($"\n开始清理文件[{CurDocHelper.Name}]...");
            CurDocHelper.StopHatchAssoc();
            CurLTypeHelper.Cleanup();
            CurLayerHelper.Cleanup();
            CurDocHelper.Audit();
            CurDocHelper.PurgeAll();
            CurLayerHelper.CleanupEnts();
            CurLayerHelper.CleanupBlocks();
            CurDocHelper.SetByLayer();
            CurDocHelper.PurgeAll();
            O_CadHelper.Quit();
        }
    }
}