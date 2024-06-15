using ACADBase;
using CommonUtils.Misc;

namespace ACADTestsOfApplication.CleanupTest
{
    public class CleanupTestDwgCommandHelper : BaseDwgCommandHelper
    {
        public CleanupTestDwgCommandHelper(string drawingFile = "") : base(drawingFile)
        {
        }

        public CleanupTestDwgCommandHelper()
        {
        }

        public OperationResult<VoidValue> TestOutput()
        {
            DocumentManagerWrapper.GetActiveEditor().WriteMessage("TestOutput");
            return OperationResult<VoidValue>.Success();
        }
    }
}