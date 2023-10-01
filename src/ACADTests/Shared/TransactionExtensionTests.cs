using System;
using System.Threading;
using ACADTests.Cleanup;
using ACADWrappers.Shared;
using Autodesk.AutoCAD.DatabaseServices;
using Domain.Shared;
using NUnit.Framework;
using TestRunnerACAD;

namespace ACADTests.Shared
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class TransactionExtensionTests
    {
        [Test]
        public void ReadDbObject()
        {
            void Action1(Database db, Transaction tr)
            {
                IDatabaseWrapper dbWrapper = new DatabaseWrapper(db);
                var ltypeTableId1 = dbWrapper.GetSymbolTableIdIntPtr(nameof(LinetypeTable));
                Type type1=null;
                tr.ReadDbObject<LinetypeTable>(ltypeTableId1, table => type1 = table.GetType());
                Assert.AreEqual(typeof(LinetypeTable), type1);
                Assert.Throws<ArgumentException>(() =>
                    tr.ReadDbObject<LayerTable>(ltypeTableId1, table => type1 = table.GetType()));
                Assert.Throws<ArgumentException>(() =>
                    tr.ReadDbObject<DBObject>(IntPtr.Zero, table => type1 = table.GetType()));
            }

            // Run the tests
            TestBaseWDb.ExecuteTestActions(CleanupTestConsts.CleanupTestDwg, Action1);
        }


      
    }
}