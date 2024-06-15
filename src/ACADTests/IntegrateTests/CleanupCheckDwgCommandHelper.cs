using ACADBase;
using CommonUtils.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACADTests.IntegrateTests
{
    public class CleanupCheckDwgCommandHelper: BaseDwgCommandHelper
    {
        public CleanupCheckDwgCommandHelper(string drawingFile = "") : base(drawingFile)
        {
        }

        public CleanupCheckDwgCommandHelper()
        {
        }

        public OperationResult<VoidValue> TestOutput()
        {
            DocumentManagerWrapper.GetActiveEditor().WriteMessage("TestOutput");
            return OperationResult<VoidValue>.Success();
        }
    }
}
