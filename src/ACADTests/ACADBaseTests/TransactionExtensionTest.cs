using System;
using ACADBase;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Moq;
using NUnit.Framework;

namespace ACADTests.ACADBaseTests
{
    [TestFixture]
    public class TransactionExtensionTest:DwgCommandDataBaseTestBase
    {
        [Test]
        public void GetObjectInvalidIdTest()
        {
            ObjectId invalidId = ObjectId.Null;
            _dwgCommandBaseActive.ExecuteDataBaseActions(db=>db.RunFuncInTransaction(tr=>tr.GetObject<DBObject>(invalidId, OpenMode.ForRead)));
            _mockMessageProvider.Verify(m => m.Error(Moq.It.IsAny<ArgumentExceptionOfInvalidId>()),
                Times.Once);
        }
        [Test]
        public void GetObjectInvalidTypeOrActionTest()
        {
            ObjectId lineId = ObjectId.Null;
            _dwgCommandBaseActive.ExecuteDataBaseActions(db => lineId=db.CreateInModelSpace<Line>());
            _dwgCommandBaseActive.ExecuteDataBaseActions(db => db.RunFuncInTransaction(tr => tr.GetObject<Circle>(lineId, OpenMode.ForRead)));
            _mockMessageProvider.Verify(m => m.Error(Moq.It.IsAny<ArgumentExceptionOfIdReferToWrongType>()),
                Times.Once);
            _dwgCommandBaseActive.ExecuteDataBaseActions(db => db.RunFuncInTransaction(tr => tr.GetObject<Line>(lineId, OpenMode.ForRead,obj=>throw _exception)));
            _mockMessageProvider.Verify(m => m.Error(_exception),
                Times.Once);
        }
    }
}