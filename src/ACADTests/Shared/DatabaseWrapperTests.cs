using System;
using System.IO;
using System.Reflection;
using System.Threading;
using ACADTests.Cleanup;
using ACADWrappers.Shared;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils;
using Domain.Shared;
using NLog;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TestRunnerACAD;
using Logger = NLog.Logger;

namespace ACADTests.Shared
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class DatabaseWrapperTests
    {
        [Test]
        public void TestPass()
        {
            NLog.LogManager.ThrowExceptions = true; // TODO Remove this when done trouble-shooting
            NLog.Common.InternalLogger.LogLevel = LogLevel.Debug;
            NLog.Common.InternalLogger.LogToConsole = true;
            NLog.Common.InternalLogger.LogFile = "c:\temp\nlog-internal.txt"; // On Linux one can use "/home/nlog-internal.txt"
            Logger logger = LogManager.GetLogger("DatabaseWrapperTests");
            logger.Info("Program started");
            LogManager.Shutdown();  // Remember to flush
            Assert.Pass("pass");
        }

        [Test]
        public void GetSymbolTableId()
        {
            void Action1(Database db, Transaction tr)
            {
                IDatabaseWrapper dbWrapper = new DatabaseWrapper(db);
                var ltypeTableId1 = dbWrapper.GetSymbolTableIdIntPtr(nameof(LinetypeTable));
                Assert.AreEqual(db.LinetypeTableId.OldIdPtr, ltypeTableId1);
                Assert.Throws<ArgumentException>(() => dbWrapper.GetSymbolTableIdIntPtr("test"));
            }

            // Run the tests
            TestBaseWDb.ExecuteTestActions(CleanupTestConsts.CleanupTestDwg, Action1);
        }


        [Test]
        public void GetSymbolTableRecordNames()
        {
            void Action1(IDatabaseWrapper dbWrapper)
            {
                var ltypeTableId1 = dbWrapper.GetSymbolTableIdIntPtr(nameof(LinetypeTable));
                var dictionary1 = dbWrapper.GetSymbolTableRecordNames(ltypeTableId1);
                Assert.IsTrue(dictionary1.ContainsKey(CleanupTestConsts.TestLType));
            }

            // Run the tests
            TestBaseWDbWrapper.ExecuteTestActions(CleanupTestConsts.CleanupTestDwg, Action1);
        }
    }
}