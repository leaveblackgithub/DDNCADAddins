using System.Threading;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using NUnit.Framework;
using TestRunnerACAD;

namespace ACADTests.Cleanup
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class CleanupTestLtype : TestBase
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
    }
}