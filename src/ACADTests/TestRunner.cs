using System;
using System.Reflection;
using ACADTests;
using Autodesk.AutoCAD.Runtime;
using NLog;
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
#if AcConsole
            TestRunnerBase.RunTestsBaseIncludes(assembly, "ACADTests.UnitTests.AcConsoleTests");
#else
            TestRunnerBase.RunTestsBaseIncludes(assembly);
#endif
        }
    }
}