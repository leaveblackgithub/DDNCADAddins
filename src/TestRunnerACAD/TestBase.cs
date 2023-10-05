using System.IO;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using NUnit.Framework;

namespace TestRunnerACAD
{
    /// <summary>
    ///     Base class for ACAD tests.
    /// </summary>
    public abstract class TestBase
    {
        protected Document DwgDocument;

        public TestBase(string drawingFile = "")
        {
            DrawingFile = drawingFile;
            if (string.IsNullOrEmpty(drawingFile))
            {
                DefaultDrawing = true;
                // Should this be executing assembly path instead?
                //drawingFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestDrawing.dwg");
            }
            else
            {
                DefaultDrawing = false;
                if (!File.Exists(drawingFile)) Assert.Fail($"Drawing file {drawingFile} does not exist.");
            }
        }

        protected string DrawingFile { get; }
        protected Database DwgDatabase { get; set; }
        protected bool DefaultDrawing { get; }

        public void Execute()
        {
            DwgDocument = Application.DocumentManager.MdiActiveDocument;

            // Lock the document and execute the test actions.
            using (DwgDocument.LockDocument())
            using (DwgDatabase = new Database(DefaultDrawing, false))
            {
                if (!string.IsNullOrEmpty(DrawingFile))
                    DwgDatabase.ReadDwgFile(DrawingFile, FileOpenMode.OpenForReadAndWriteNoShare, true, null);
                RunTestActions();
                var oldDb = HostApplicationServices.WorkingDatabase;
                HostApplicationServices.WorkingDatabase = DwgDatabase; // change to the current database.


                // Change the database back to the original.
                HostApplicationServices.WorkingDatabase = oldDb;
            }
        }

        protected abstract void RunTestActions();
    }
}