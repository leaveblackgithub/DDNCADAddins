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

        public override FuncResult ExecuteMain()
        {
            FuncResult result=CommandDataBaseHelper.CreateInCurrentSpace<Line>(out var handleValue);
            return CommandDataBaseHelper.TryGetObjectId(handleValue, out var _);
        }
    }
}