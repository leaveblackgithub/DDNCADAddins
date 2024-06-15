using System.Reflection;
using ACADTestsOfUnit;
using Autodesk.AutoCAD.Runtime;
using TestRunnerACAD;

[assembly: CommandClass(typeof(TestRunner))]

namespace ACADTestsOfUnit
{
    public class TestRunner
    {
        //TODO Modify TestLoader.scr and debug path programatically
        [CommandMethod("RunTests", CommandFlags.Session)]
        public static void RunTests()
        {
            var assembly = Assembly.GetExecutingAssembly();
#if AcConsoleTest
            TestRunnerBase.RunTestsBaseIncludes(assembly, "ACADTestsOfUnit.UnitTests.AcConsoleTests");
#else
            TestRunnerBase.RunTestsBaseIncludes(assembly);
#endif
        }
    }
}