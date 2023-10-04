using System;
using ACADTests.Cleanup;
using ACADWrappers.Shared;
using Autodesk.AutoCAD.DatabaseServices;
using Domain.Shared;
using NLog;
using TestRunnerACAD;

namespace ACADTests.Shared
{
    /// <summary>
    ///     Executes a list of delegate actions
    /// </summary>
    /// <param name="drawingFile">Path to the test drawing file.</param>
    /// <param name="testActions">Test actions to execute.</param>
    public class TestBaseWDbWrapper : TestBase
    {
        public TestBaseWDbWrapper(string drawingFile,
            params Action<IDatabaseWrapper>[] testActions) : base(drawingFile)
        {
            TestActions = testActions;
        }

        public Action<IDatabaseWrapper>[] TestActions { get; }

        protected override void RunTestActions()
        {
            Exception exception = null;
            IDatabaseWrapper dbWrapper = new DatabaseWrapper(Db);
            foreach (var testAction in TestActions)
            {
                try
                {
                    // Execute the test action.
                    testAction(dbWrapper);
                }
                catch (Exception e)
                {
                    LogManager.GetCurrentClassLogger().Error(e);
                    exception = e;

                    break;
                }
            }

            // From CADBloke
            // Throw exception outside of transaction.
            // Sometimes Autocad crashes when exception throws.
            if (exception != null) throw exception;
        }

        public static void ExecuteTestActions(string drawingFile = "",
            params Action<IDatabaseWrapper>[] testActions)
        {
            TestBaseWDbWrapper testBaseWDbWrapper = new TestBaseWDbWrapper(drawingFile, testActions);
            testBaseWDbWrapper.Execute();
        }
    }
}