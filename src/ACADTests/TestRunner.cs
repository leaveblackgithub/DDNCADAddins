using System.Reflection;
using ACADTests;
using Autodesk.AutoCAD.Runtime;
using TestRunnerACAD;

[assembly: CommandClass(typeof(TestRunner))]

namespace ACADTests
{
    public class TestRunner
    {
        [CommandMethod("RunTests", CommandFlags.Session)]
        public static void RunTests()
        {
            var assembly = Assembly.GetExecutingAssembly();
            TestRunnerBase.RunTestsBase(assembly);
        }
    }
}