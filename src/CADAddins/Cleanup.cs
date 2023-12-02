using Autodesk.AutoCAD.Runtime;
using CADAddins;
using CADAddins.Archive;
using CADAddins.LibsOfCleanup;

[assembly: CommandClass(typeof(Cleanup))]

namespace CADAddins
{
    /// <summary>
    /// TODO:BUG1.有些线形会被当成"ByBlock"
    /// TODO:BUG2. 关闭文件会崩溃
    /// TODO:BUG3. 测试文件drawing to cleanup有图层没清理干净
    /// TODO:BUG4. HIDDEN5线形被误清理。
    /// TODO: 1. 自动弹出文本窗口，排列窗体
    /// TODO: 2. 完成INTEGRATE TEST
    /// TODO: 3. 重构代码，增加UNIT TEST，增加LOG，增加脱离ACAD的单独测试
    /// TODO: 4. 增加按颜色拆分的选项
    /// TODO: 5. 如果不拆分颜色，要SETBYLAYER
    /// 
    /// </summary>
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