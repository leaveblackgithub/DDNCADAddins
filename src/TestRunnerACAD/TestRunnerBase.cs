using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnitLite;

namespace TestRunnerACAD
{
    public class TestRunnerBase
    {
        private static List<string> _nunitArgs;

        public static void Initiate()
        {
            var directoryReportUnit = TestRunnerConsts.ReportToolFolderName;
            if (!Directory.Exists(directoryReportUnit)) return;
            var fileInputXml = Path.Combine(directoryReportUnit, TestRunnerConsts.ReportNunitXml);
            if (File.Exists(fileInputXml))
                File.Delete(fileInputXml);
            _nunitArgs = new List<string>
            {
                //"--trace=verbose",
                "--result=" + fileInputXml
            };
        }

        private static void RunTests(Assembly assembly)
        {
            new AutoRun(assembly).Execute(_nunitArgs.ToArray());

            TestReportGenerator.CreateTestReport();
        }

        //TODO This can only be reference as project not dll.
        public static void RunTestsBaseIncludes(Assembly assembly, string prefilter = "")
        {
            Initiate();
            if (!string.IsNullOrEmpty(prefilter)) _nunitArgs.Add($"--prefilter={prefilter}");

            RunTests(assembly);
        }

        //TODO This can only be reference as project not dll.
        public static void RunTestsBaseExcludes(Assembly assembly, string exclude = "")
        {
            Initiate();
            if (!string.IsNullOrEmpty(exclude)) _nunitArgs.Add($"--exclude={exclude}");

            RunTests(assembly);
        }
    }
}