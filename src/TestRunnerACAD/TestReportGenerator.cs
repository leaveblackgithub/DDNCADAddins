﻿using System.Diagnostics;
using System.IO;

namespace TestRunnerACAD
{
    public class TestReportGenerator
    {
        public static void CreateTestReport(string assemblyPath)
        {
            if (assemblyPath == null)
                return;

            var directoryReportUnit = Path.Combine(assemblyPath, TestRunnerConsts.ReportToolFolderName);
            Directory.CreateDirectory(directoryReportUnit);
            var fileInputXml = Path.Combine(directoryReportUnit, TestRunnerConsts.ReportNunitXml);
            if (!File.Exists(fileInputXml))
                return;
            var fileOutputHtml = Path.Combine(directoryReportUnit, TestRunnerConsts.ReportOutputHtml);
            if (File.Exists(fileOutputHtml))
                File.Delete(fileOutputHtml);
            var generatorReportUnit = Path.Combine(assemblyPath, TestRunnerConsts.ReportToolFolderName,
                TestRunnerConsts.ReportToolFileName);
            //The extentreports-dotnet-cli deprecates ReportUnit. Can only define output folder and  export to default index.html
            CreateHtmlReport(fileInputXml, directoryReportUnit, generatorReportUnit);
            OpenHtmlReport(fileOutputHtml);
        }

        /// <summary>
        ///     Opens a HTML report with the default viewer.
        /// </summary>
        /// <param name="fileName"></param>
        private static void OpenHtmlReport(string fileName)
        {
            if (!File.Exists(fileName))
                return;
            using (var process = new Process())
            {
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.RedirectStandardOutput = false;
                process.StartInfo.FileName = fileName;
                process.Start();
            }
        }

        /// <summary>
        ///     Creates a HTML report based on the NUnit XML report.
        /// </summary>
        /// <param name="inputFile">The NUnit XML file.</param>
        /// <param name="outputFolder">The output HTML report file.</param>
        /// <param name="reportUnitPath">Path to the ReportUnit executable.</param>
        private static void CreateHtmlReport(string inputFile, string outputFolder, string reportUnitPath)
        {
            if (!File.Exists(inputFile))
                return;

            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            using (var process = new Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.FileName = reportUnitPath;
                //extent -i results/nunit.xml -o results/ -r v3html
                process.StartInfo.Arguments = $" \"-i\" \"{inputFile}\" \"-o\" \"{outputFolder}\" \"-r\" \"v3html\"";

                process.Start();
                process.WaitForExit();
            }
        }
    }
}