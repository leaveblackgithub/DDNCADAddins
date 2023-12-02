using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnitLite;

namespace TestRunnerACAD
{
    public class TestRunnerBase
    {
        //TODO This can only be reference as project not dll.
        public static void RunTestsBase(Assembly assembly,string prefilter="")
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
            };
            if (!string.IsNullOrEmpty(prefilter)) nunitArgs.Add($"--prefilter={prefilter}");

            new AutoRun(assembly).Execute(nunitArgs.ToArray());

            TestReportGenerator.CreateTestReport();
        }
    }
}