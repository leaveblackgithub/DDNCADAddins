using ACADBase;
using ACADTestsOfApplication.CleanupTest;
using Autodesk.AutoCAD.Runtime;
using CommonUtils.Misc;

[assembly: CommandClass(typeof(CleanupTestCommand))]

namespace ACADTestsOfApplication.CleanupTest
{
    public class CleanupTestCommand 
    {
        //private LayerHelper _curLayerHelper;
        //private LTypeHelper _curLTypeHelper;


        //public LayerHelper CurLayerHelper =>
        //    _curLayerHelper ?? (_curLayerHelper =
        //        new LayerHelper(AcCurDb.LayerTableId, O_CurDocHelper, CurLTypeHelper));

        //public LTypeHelper CurLTypeHelper =>
        //    _curLTypeHelper ?? (_curLTypeHelper =
        //        new LTypeHelper(AcCurDb.LinetypeTableId, O_CurDocHelper));

        [CommandMethod(nameof(CleanupTestCommand))]
        public  void RunCommand()
        {
            BaseDwgCommandHelper.ExecuteCustomCommands<CleanupTestDwgCommandHelper>("", TestOutput);
            //O_CurEditorHelper.WriteMessage($"\nCleanup Result Check [{O_CurDocHelper.Name}]...");
            //CurLTypeHelper.Check();
            //CurLayerHelper.Check();
            // O_CurEditorHelper.WriteMessage($"\n{LayerHelper.GetNewName(@"7715_SP01$0$Text & Dims", "Continuous")}");
            //O_CadHelper.Quit();
        }

        protected OperationResult<VoidValue> TestOutput(CleanupTestDwgCommandHelper dwgCommandHelper)
        {
            return dwgCommandHelper.TestOutput();
        }
    }
}