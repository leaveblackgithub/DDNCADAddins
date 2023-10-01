using System.IO;
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
        protected Database Db { get; set; }
        protected bool DefaultDrawing { get; }

        public void Execute()
        {
            var document = Application.DocumentManager.MdiActiveDocument;

            // Lock the document and execute the test actions.
            using (document.LockDocument())
            using (Db = new Database(DefaultDrawing, false))
            {
                if (!string.IsNullOrEmpty(DrawingFile))
                    Db.ReadDwgFile(DrawingFile, FileOpenMode.OpenForReadAndWriteNoShare, true, null);
                RunTestActions();
                var oldDb = HostApplicationServices.WorkingDatabase;
                HostApplicationServices.WorkingDatabase = Db; // change to the current database.


                // Change the database back to the original.
                HostApplicationServices.WorkingDatabase = oldDb;
            }
        }

        protected abstract void RunTestActions();
    }
}