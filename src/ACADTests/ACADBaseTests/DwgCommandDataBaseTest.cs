using System;
using System.Threading;
using ACADBase;
using Autodesk.AutoCAD.DatabaseServices;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace ACADTests.ACADBaseTests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class DwgCommandDataBaseTest : DwgCommandDataBaseTestBase
    {
        private long _lineId;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _lineId = 0;
        }
        //Use a new drawing

        private void AddLine(Database db)
        {
            _lineId = 0;
            var objectId = db.CreateInModelSpace<Line>();

            _lineId = objectId.Handle.Value;
        }

        private void CheckLine(Database db)
        {
            //Check in another transaction if the line was created

            if (!db.TryGetObjectId(new Handle(_lineId), out _)) Assert.Fail("Line didn't created");
            _lineId = 0;
        }

        [Test]
        public void TestAddLineInTestDwg()
        {
            // Run the tests
            _dwgCommandBaseTest.ExecuteDataBaseActions(AddLine, CheckLine);
        }

        [Test]
        public void TestAddLineInActiveDwg()
        {
            // Run the tests
            _dwgCommandBaseActive.ExecuteDataBaseActions(AddLine, CheckLine);
        }

        [Test]
        public void TestWrongDwgName()
        {
            Assert.Throws<ArgumentException>(() => new DwgCommandBase(
                @"D:\NonExisting.dwg"));
        }

        [Test]
        public void TestExceptionInRunCommandActions()
        {
            var dwgCommandBaseMockProtected = new Mock<DwgCommandBase>("", _mockMessageProvider.Object);
            dwgCommandBaseMockProtected.Protected().Setup("RunCommandActions").Throws(_exception);
            dwgCommandBaseMockProtected.Object.ExecuteDataBaseActions(emptyDbAction);
            Assert.Throws<MockException>(() => _mockMessageProvider.Verify(m => m.Error(new Exception()), Times.Once));
            _mockMessageProvider.Verify(m => m.Error(_exception), Times.Once);
        }

        [Test]
        public void TestExceptionInExecuteDatabase()
        {
            _dwgCommandBaseActive.ExecuteDataBaseActions(db => throw _exception);
            Assert.Throws<MockException>(() => _mockMessageProvider.Verify(m => m.Error(new Exception()), Times.Once));
            _mockMessageProvider.Verify(m => m.Error(_exception), Times.Once);
        }
    }
}