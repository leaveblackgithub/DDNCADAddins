using System.Diagnostics;
using System.IO;
using System.Reflection;
using Autodesk.AutoCAD.Runtime;
using TestRunnerACAD;

[assembly: ExtensionApplication(typeof(TestPlugin))]

namespace TestRunnerACAD
{
    public class TestPlugin : IExtensionApplication
    {
        public void Initialize()
        {
            // Don't need to do anything here.
        }

        public void Terminate()
        {
            // var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //
            // TestReportGenerator.CreateTestReport(assemblyPath);
        }
    }
}