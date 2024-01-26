using ACADBase;
using Autodesk.AutoCAD.DatabaseServices;
using NUnit.Framework;

namespace ACADTests.UnitTests.AcConsoleTests
{
    [TestFixture]
    public class TransactionExtensionTest : DwgCommandHelperTestBase
    {
        [Test]
        public void GetObjectInvalidIdTest()
        {
            var invalidId = ObjectId.Null;
            DwgCommandHelperActive.ExecuteDataBaseActions(db =>
                db.RunFuncInTransaction(tr => tr.GetObject<DBObject>(invalidId, OpenMode.ForRead)));
            MsgProviderVerifyExTypeOnce<ArgumentExceptionOfInvalidId>();
        }

        [Test]
        public void GetObjectInvalidTypeOrActionTest()
        {
            var lineId = ObjectId.Null;
            DwgCommandHelperActive.ExecuteDataBaseActions(db => lineId = db.CreateInModelSpace<Line>());
            DwgCommandHelperActive.ExecuteDataBaseActions(db =>
                db.RunFuncInTransaction(tr => tr.GetObject<Circle>(lineId, OpenMode.ForRead)));
            MsgProviderVerifyExTypeOnce<ArgumentExceptionOfIdReferToWrongType>();
            DwgCommandHelperActive.ExecuteDataBaseActions(db =>
                db.RunFuncInTransaction(tr => tr.GetObject<Line>(lineId, OpenMode.ForRead, obj => throw ExInitInBase)));
            MsgProviderShowExInitInBaseOnce();
        }
    }
}