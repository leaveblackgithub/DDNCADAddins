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
            // var _dwgCommandBaseTest = new DwgCommandBase(
            //     @"D:\leaveblackgithub\DDNCADAddinsForRevitImport\src\ACADTests\TestDrawing.dwg");
            // _dwgCommandBaseTest.Execute(ExceptionMethod);
            var assembly = Assembly.GetExecutingAssembly();
            TestRunnerBase.RunTestsBase(assembly);
        }

        // private static void ExceptionMethod(Database db, Transaction tr)
        // {
        //         throw new System.Exception("Test Exception in TransactionAction");
        //   
        // }
    }
}