using CommonUtils.Misc;

namespace ACADTests.UnitTests.AcConsoleTests
{
    //TODO CREATE SUB CLASS FOR TEST OF DWGCOMMANDHELPER
    public class DwgCommandHelperTestBase
    {
        protected const string TestDrawingName = "TestDrawing.dwg";
        protected const string TestTxtName = "TestTxt.txt";
        protected const string FakeDrawingName = "FakeDrawing.dwg";

        protected const string TestFolder = @"D:\leaveblackgithub\DDNCADAddinsForRevitImport\src\ACADTests\";


        protected string TestDrawingPath => TestFolder + TestDrawingName;
        protected string TestTxtPath => TestFolder + TestTxtName;
        protected string FakeDrawingPath => TestFolder + FakeDrawingName;


        protected OperationResult<VoidValue> TestAddingLines(DwgCommandHelperOfTest dwgCommandHelper)
        {
            return dwgCommandHelper.TestAddingLines();
        }
        
    }
}