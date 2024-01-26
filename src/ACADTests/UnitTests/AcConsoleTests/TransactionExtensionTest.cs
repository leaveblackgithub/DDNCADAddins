using System;
using ACADBase;
using Autodesk.AutoCAD.DatabaseServices;
using Moq;
using NUnit.Framework;

namespace ACADTests.UnitTests.AcConsoleTests
{
    [TestFixture]
    public class TransactionExtensionTest:DwgCommandDataBaseTestBase
    {
        [Test]
        public void GetObjectInvalidIdTest()
        {
            ObjectId invalidId = ObjectId.Null;
            DwgCommandHelperActive.ExecuteDataBaseActions(db=>db.RunFuncInTransaction(tr=>tr.GetObject<DBObject>(invalidId, OpenMode.ForRead)));
            _mockMessageProvider.Verify(m => m.Error(Moq.It.IsAny<ArgumentExceptionOfInvalidId>()),
                Times.Once);
        }
        [Test]
        public void GetObjectInvalidTypeOrActionTest()
        {
            ObjectId lineId = ObjectId.Null;
            DwgCommandHelperActive.ExecuteDataBaseActions(db => lineId=db.CreateInModelSpace<Line>());
            DwgCommandHelperActive.ExecuteDataBaseActions(db => db.RunFuncInTransaction(tr => tr.GetObject<Circle>(lineId, OpenMode.ForRead)));
            _mockMessageProvider.Verify(m => m.Error(Moq.It.IsAny<ArgumentExceptionOfIdReferToWrongType>()),
                Times.Once);
            DwgCommandHelperActive.ExecuteDataBaseActions(db => db.RunFuncInTransaction(tr => tr.GetObject<Line>(lineId, OpenMode.ForRead,obj=>throw ExInitInBase)));
            _mockMessageProvider.Verify(m => m.Error(ExInitInBase),
                Times.Once);
        }
    }
}