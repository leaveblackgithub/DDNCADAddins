using System;
using System.Collections.Generic;
using ACADTests.Cleanup;
using Autodesk.AutoCAD.DatabaseServices;
using Domain.Shared;
using NUnit.Framework;
using System.Threading;
using ACADWrappers.Shared;
using Autodesk.AutoCAD.ApplicationServices.Core;
using TestRunnerACAD;

namespace ACADTests.Shared
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class DatabaseWrapperTests : TestBase
    {
        [Test]
        public void GetSymbolTableId()
        {
            void Action1(Database db, Transaction tr)
            {
                IDatabaseWrapper dbWrapper = new DatabaseWrapper(db);
                IntPtr ltypeTableId1 = dbWrapper.GetSymbolTableIdIntPtr(nameof(LinetypeTable));
                Assert.AreEqual(db.LinetypeTableId.OldIdPtr, ltypeTableId1);
                Assert.AreEqual(IntPtr.Zero,dbWrapper.GetSymbolTableIdIntPtr("test"));
            }
            // Run the tests
            ExecuteTestActions(CleanupTestConsts.CleanupTestDwg, Action1);
        }


        [Test]
        public void GetSymbolTableRecordNames()
        {
            void Action1(Database db, Transaction tr)
            {
                IDatabaseWrapper dbWrapper = new DatabaseWrapper(db);
                IntPtr ltypeTableId1 = dbWrapper.GetSymbolTableIdIntPtr(nameof(LinetypeTable));
                Dictionary<string,IntPtr> dictionary1=dbWrapper.GetSymbolTableRecordNames(ltypeTableId1);
                Dictionary<string, IntPtr> dictionary2 = dbWrapper.GetSymbolTableRecordNames(dbWrapper.GetSymbolTableIdIntPtr("test"));
                Assert.IsTrue(dictionary1.ContainsKey(CleanupTestConsts.TestLType));
                Assert.AreEqual(0,dictionary2.Count);
            }
            // Run the tests
            ExecuteTestActions(CleanupTestConsts.CleanupTestDwg, Action1);
        }

    }
}