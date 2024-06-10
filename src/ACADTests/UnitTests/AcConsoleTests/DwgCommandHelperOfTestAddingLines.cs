using ACADBase;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.Misc;

namespace ACADTests.UnitTests.AcConsoleTests
{
    public class DwgCommandHelperOfTestAddingLines:DwgCommandHelperBase
    {
        public DwgCommandHelperOfTestAddingLines(string drawingFile = "", IMessageProvider messageProvider = null) : base(drawingFile, messageProvider)
        {
        }

        public DwgCommandHelperOfTestAddingLines():base()
        {
        }

        public override OperationResult<VoidValue> CustomExecute()
        {
            var resultHandleValue =CommandDataBaseHelper.CreateInCurrentSpace<Line>();
            var resultId = resultHandleValue.Then<HandleValue, ObjectId>(() =>
                CommandDataBaseHelper.TryGetObjectId(resultHandleValue.ReturnValue));
            return resultId.IsSuccess ? OperationResult<VoidValue>.Success() : OperationResult<VoidValue>.Failure(resultId.ErrorMessage);
        }
    }
}