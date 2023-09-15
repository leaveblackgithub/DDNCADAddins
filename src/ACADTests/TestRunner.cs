using System.IO;
using System.Reflection;
using ACADTest;
using Autodesk.AutoCAD.Runtime;
using TestRunnerACAD;

[assembly: CommandClass(typeof(TestRunner))]

namespace ACADTest
{
    public class TestRunner : TestRunnerBase
    {
        [CommandMethod("RunTests", CommandFlags.Session)]
        public void RunTests()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var directoryPlugin = Path.GetDirectoryName(assembly.Location);
            RunTestsBase(assembly, directoryPlugin);
        }
    }
}