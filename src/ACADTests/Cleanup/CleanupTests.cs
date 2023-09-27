using System.Threading;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Domain.Shared;
using NUnit.Framework;
using TestRunnerACAD;

namespace ACADTests.Cleanup
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class CleanupTests : TestBase
    {
        [Test]
        public void ReadLtypes()
        {

            void Action1(Database db, Transaction tr)
            {
                //get all line types in the database
                using (var ltypes = db.LinetypeTableId.GetObject(OpenMode.ForRead) as LinetypeTable)
                {
                    Assert.IsTrue(ltypes.Has(CleanupTestConsts.TestLType));
                }
            }
            // Run the tests
            ExecuteTestActions(CleanupTestConsts.CleanupTestDwg, Action1);
        }
        [Test]
        public void SetSystemVariable()
        {
            void Action1(Database db, Transaction tr)
            {
                Assert.AreEqual(1, Application.GetSystemVariable(ACADConsts.Hpassoc));
                Application.SetSystemVariable(ACADConsts.Hpassoc, 0);
            }

            void Action2(Database db, Transaction tr)
            {
                Assert.AreEqual(0, Application.GetSystemVariable(ACADConsts.Hpassoc));
            }
            

            // Run the tests
            ExecuteTestActions(CleanupTestConsts.CleanupTestDwg, Action1, Action2);
        }
    }
}