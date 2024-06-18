using System.IO;
using CommonUtils.Misc;

namespace ACADTestsOfUnit.UnitTests.AcConsoleTests
{
    //TODO CREATE SUB CLASS FOR TEST OF DWGCOMMANDHELPER
    public class DwgCommandHelperTestBase
    {
        protected const string TestDrawingName = "TestDrawing.dwg";
        protected const string TestTxtName = "TestTxt.txt";
        protected const string FakeDrawingName = "FakeDrawing.dwg";

        protected const string TestFolder = @"D:\leaveblackgithub\DDNCADAddinsForRevitImport\examples\";


        protected string TestDrawingPath => Path.Combine(TestFolder, TestDrawingName);
        protected string TestTxtPath => Path.Combine(TestFolder, TestTxtName);
        protected string FakeDrawingPath => Path.Combine(TestFolder, FakeDrawingName);

        
    }
}