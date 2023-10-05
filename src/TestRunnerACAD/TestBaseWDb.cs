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
                using (var tr = DwgDatabase.TransactionManager.StartTransaction())
                {
                    try
                    {
                        // Execute the test action.
                        testAction(DwgDatabase, tr);
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
            var testBase = new TestBaseWDb(drawingFile, testActions);
            testBase.Execute();
        }
    }
}