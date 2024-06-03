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

        public override CommandResult Execute()
        {
            CommandResult result=CommandDataBaseHelper.CreateInCurrentSpace<Line>(out var handleValue);
            if (!CommandDataBaseHelper.TryGetObjectId(handleValue, out _)) result.Cancel();
            return result;
        }
    }
}