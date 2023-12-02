using System.Reflection;
using ACADTests;
using Autodesk.AutoCAD.Runtime;
using TestRunnerACAD;

[assembly: CommandClass(typeof(TestRunner))]

namespace ACADTests
{
    public class TestRunner
    {
        //TODO Modify TestLoader.scr and debug path programatically
        [CommandMethod("RunTests", CommandFlags.Session)]
        public static void RunTests()
        {
            // var DwgCommandHelperTest = new DwgCommandHelper(
            //     @"D:\leaveblackgithub\DDNCADAddinsForRevitImport\src\ACADTests\TestDrawing.dwg");
            // DwgCommandHelperTest.Execute(ExceptionMethod);
            var assembly = Assembly.GetExecutingAssembly();
#if AcConsole
            TestRunnerBase.RunTestsBase(assembly, "ACADTests.UnitTests.AcConsoleTests");
#else
            TestRunnerBase.RunTestsBase(assembly, "ACADTests.UnitTests.ACADAppTests");
#endif
        }

        // private static void ExceptionMethod(Database db, Transaction tr)
        // {
        //         throw new System.Exception("Test Exception in TransactionAction");
        //   
        // }
    }
}