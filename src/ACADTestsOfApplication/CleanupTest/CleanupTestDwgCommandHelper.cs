using ACADBase;
using ACADBaseOfApplication;
using CommonUtils.Misc;

namespace ACADTestsOfApplication.CleanupTest
{
    public class CleanupTestDwgCommandHelper : AppBaseDwgCommandHelper
    {
        public CleanupTestDwgCommandHelper(string drawingFile = "") : base(drawingFile)
        {
        }

        public CleanupTestDwgCommandHelper()
        {
        }

        public OperationResult<VoidValue> TestOutput()
        {
            ActiveEditorWrapper.WriteMessage("TestOutput3");
            return OperationResult<VoidValue>.Success();
        }
    }
}