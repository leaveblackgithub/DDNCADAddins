using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnitLite;

namespace TestRunnerACAD
{
    public class TestRunnerBase
    {
        public static void RunTestsBase(Assembly assembly)
        {
            var directoryReportUnit = TestRunnerConsts.ReportToolFolderName;
            if (!Directory.Exists(directoryReportUnit)) return;
            var fileInputXml = Path.Combine(directoryReportUnit, TestRunnerConsts.ReportNunitXml);
            if (File.Exists(fileInputXml))
                File.Delete(fileInputXml);
            var nunitArgs = new List<string>
            {
                "--trace=verbose",
                "--result=" + fileInputXml
            }.ToArray();

            new AutoRun(assembly).Execute(nunitArgs);

            TestReportGenerator.CreateTestReport();
        }
    }
}