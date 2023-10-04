using System;
using Autodesk.AutoCAD.DatabaseServices;
using NLog;

namespace TestRunnerACAD
{
    /// <summary>
    ///     Executes a list of delegate actions
    /// </summary>
    /// <param name="drawingFile">Path to the test drawing file.</param>
    /// <param name="testActions">Test actions to execute.</param>
    public class TestBaseWDb : TestBase
    {
        public TestBaseWDb(string drawingFile,
            params Action<Database, Transaction>[] testActions) : base(drawingFile)
        {
            TestActions = testActions;
        }

        public Action<Database, Transaction>[] TestActions { get; }

        protected override void RunTestActions()
        {
            Exception exception = null;
            foreach (var testAction in TestActions)
                using (var tr = Db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        // Execute the test action.
                        testAction(Db, tr);
                        tr.Commit();
                    }
                    catch (Exception e)
                    {
                        exception = e;
                        LogManager.GetCurrentClassLogger().Error(e);
                        tr.Abort();
                        break;
                    }
                }

            // From CADBloke
            // Throw exception outside of transaction.
            // Sometimes Autocad crashes when exception throws.
            if (exception != null) throw exception;
        }

        /// <summary>
        ///     Executes a list of delegate actions
        /// </summary>
        /// <param name="drawingFile">Path to the test drawing file.</param>
        /// <param name="testActions">Test actions to execute.</param>
        public static void ExecuteTestActions(string drawingFile = "",
            params Action<Database, Transaction>[] testActions)
        {
            TestBaseWDb testBase=new TestBaseWDb(drawingFile,testActions);
            testBase.Execute();
            // bool defaultDrawing;
            // if (string.IsNullOrEmpty(drawingFile))
            // {
            //     defaultDrawing = true;
            //     // Should this be executing assembly path instead?
            //     //drawingFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestDrawing.dwg");
            // }
            // else
            // {
            //     defaultDrawing = false;
            //     if (!File.Exists(drawingFile)) Assert.Fail($"Drawing file {drawingFile} does not exist.");
            // }
            //
            // Exception exception = null;
            // var document = Application.DocumentManager.MdiActiveDocument;
            //
            // // Lock the document and execute the test actions.
            // using (document.LockDocument())
            // using (var db = new Database(defaultDrawing, false))
            // {
            //     if (!string.IsNullOrEmpty(drawingFile))
            //         db.ReadDwgFile(drawingFile, FileOpenMode.OpenForReadAndWriteNoShare, true, null);
            //
            //     var oldDb = HostApplicationServices.WorkingDatabase;
            //     HostApplicationServices.WorkingDatabase = db; // change to the current database.
            //
            //     foreach (var testAction in testActions)
            //         using (var tr = db.TransactionManager.StartTransaction())
            //         {
            //             try
            //             {
            //                 // Execute the test action.
            //                 testAction(db, tr);
            //             }
            //             catch (Exception e)
            //             {
            //                 exception = e;
            //                 
            //                 break;
            //             }
            //             finally{ tr.Commit(); }
            //             
            //         }
            //
            //     // Change the database back to the original.
            //     HostApplicationServices.WorkingDatabase = oldDb;
            // }
            //
            // // From CADBloke
            // // Throw exception outside of transaction.
            // // Sometimes Autocad crashes when exception throws.
            // if (exception != null) throw exception;
        }
    }
}