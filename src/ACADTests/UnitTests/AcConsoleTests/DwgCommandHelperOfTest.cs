using System.Windows.Forms;
using ACADBase;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.Misc;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace ACADTests.UnitTests.AcConsoleTests
{
    public class DwgCommandHelperOfTest : DwgCommandHelperBase
    {
        public DwgCommandHelperOfTest(string drawingFile = "") :
            base(drawingFile)
        {
        }

        public DwgCommandHelperOfTest()
        {
        }

        public OperationResult<VoidValue> TestAddingLines()
        {
            var resultHandleValue = CommandDataBaseHelper.CreateInCurrentSpace<Line>();
            var resultId = resultHandleValue.Then(() =>
                CommandDataBaseHelper.TryGetObjectId(resultHandleValue.ReturnValue));
            return resultId.IsSuccess
                ? OperationResult<VoidValue>.Success()
                : OperationResult<VoidValue>.Failure(resultId.ErrorMessage);
        }
        
    }
}