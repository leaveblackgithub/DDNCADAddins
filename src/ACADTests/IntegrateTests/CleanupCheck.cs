using ACADBase;
using ACADTests.IntegrateTests;
using ACADTests.UnitTests.AcConsoleTests;
using Autodesk.AutoCAD.Runtime;
using CADAddins.Archive;
using CADAddins.LibsOfCleanup;
using CommonUtils.Misc;

[assembly: CommandClass(typeof(CleanupCheck))]

namespace ACADTests.IntegrateTests
{
    public class CleanupCheck : O_CommandBase
    {
        //private LayerHelper _curLayerHelper;
        //private LTypeHelper _curLTypeHelper;


        //public LayerHelper CurLayerHelper =>
        //    _curLayerHelper ?? (_curLayerHelper =
        //        new LayerHelper(AcCurDb.LayerTableId, O_CurDocHelper, CurLTypeHelper));

        //public LTypeHelper CurLTypeHelper =>
        //    _curLTypeHelper ?? (_curLTypeHelper =
        //        new LTypeHelper(AcCurDb.LinetypeTableId, O_CurDocHelper));

        [CommandMethod("CleanupCheck")]
        public override void RunCommand()
        {
            BaseDwgCommandHelper.ExecuteCustomCommands<CleanupCheckDwgCommandHelper>("",TestOutput);
            //O_CurEditorHelper.WriteMessage($"\nCleanup Result Check [{O_CurDocHelper.Name}]...");
            //CurLTypeHelper.Check();
            //CurLayerHelper.Check();
            // O_CurEditorHelper.WriteMessage($"\n{LayerHelper.GetNewName(@"7715_SP01$0$Text & Dims", "Continuous")}");
            //O_CadHelper.Quit();
        }
        protected OperationResult<VoidValue> TestOutput(CleanupCheckDwgCommandHelper dwgCommandHelper)
        {
            return dwgCommandHelper.TestOutput();
        }
    }
}