using System.Reflection;
using ACADBase;
using ACADTests;
using ACADTests.UnitTests.AcConsoleTests;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.Runtime;
using CommonUtils.Misc;
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
            var assembly = Assembly.GetExecutingAssembly();
#if AcConsoleTest
            TestRunnerBase.RunTestsBaseIncludes(assembly, "ACADTests.UnitTests.AcConsoleTests");
#else
            TestRunnerBase.RunTestsBaseIncludes(assembly);
#endif
        }
        
    }
}