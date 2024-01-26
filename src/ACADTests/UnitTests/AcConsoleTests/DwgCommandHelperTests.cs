using System;
using System.Text;
using System.Threading;
using ACADBase;
using Autodesk.AutoCAD.DatabaseServices;
using Moq;
using Moq.Protected;
using NLog;
using NUnit.Framework;

namespace ACADTests.UnitTests.AcConsoleTests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class DwgCommandHelperTests : DwgCommandHelperTestBase
    {
        private long _lineId;
        private TestException _exInitInContext;

        private TestException ExInitInContext =>
            _exInitInContext ?? (_exInitInContext = new TestException(nameof(ExInitInContext)));

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
            DwgCommandHelperOfTestDwg.ExecuteDataBaseActions(AddLine, CheckLine);
        }

        [Test]
        public void TestAddLineInActiveDwg()
        {
            // Run the tests
            DwgCommandHelperActive.ExecuteDataBaseActions(AddLine, CheckLine);
        }

        [Test]
        public void TestWrongDwgName()
        {
            Assert.Throws<ArgumentException>(() => new DwgCommandHelper(
                @"D:\NonExisting.dwg"));
        }


        [Test]
        public void TestWritingExMsgNotThrowingInRunCommandActions()
        {
            DwgCommandBaseMockProtected.Protected().Setup("RunCommandActions").Throws(ExInitInBase);
            DwgCommandBaseMockProtected.Object.ExecuteDataBaseActions(EmptyDbAction);
            ExampleShowsVerifyCheckingExactlySameObject();
            MsgProviderShowExInitInBaseOnce();
            ExampleOfVerifyOnlyWorkOnce();
        }

        private void ExampleOfVerifyOnlyWorkOnce()
        {
            Assert.Throws<MockException>(MsgProviderShowExInitInBaseOnce);
        }

        private void ExampleShowsVerifyCheckingExactlySameObject()
        {
            //Example shows parameter verify should be exactly the same object.
            Assert.Throws<MockException>(() =>
                MsgProviderMockInitInBase.Verify(m => m.Error(new Exception()), Times.Once));
        }

        [Test]
        public void TestWritingExMsgWMsgBox()
        {
            DwgCommandHelperOfMsgBox.WriteMessage("Testing MsgboxAsProvider");
        }


        [Test]
        public void TestExceptionScopeAndTrack()
        {
            var msgProviderMock = new Mock<IMessageProvider>();
            var exscopeandtrack = new StringBuilder();
            msgProviderMock.Setup(m => m.Error(It.IsAny<Exception>()))
                .Callback<Exception>(e => exscopeandtrack.AppendLine($"[{e.Message}]:{e.StackTrace}"));
            var dwgCommandHelperOfRecordingExScopeAndTrack = new DwgCommandHelper("", msgProviderMock.Object);
            var exInitInMethod = new TestException("exInitInmethod");
            dwgCommandHelperOfRecordingExScopeAndTrack.ExecuteDataBaseActions(db => throw exInitInMethod);
            dwgCommandHelperOfRecordingExScopeAndTrack.ExecuteDataBaseActions(db => throw ExInitInContext);
            dwgCommandHelperOfRecordingExScopeAndTrack.ExecuteDataBaseActions(db => throw ExInitInBase);
            LogManager.GetCurrentClassLogger().Info(exscopeandtrack.ToString());
            ExceptionsOfDifScopeHasSameStackTraceFrLine2(exscopeandtrack);
        }

        private static void ExceptionsOfDifScopeHasSameStackTraceFrLine2(StringBuilder exscopeandtrack)
        {
            var stackTraceList = exscopeandtrack.ToString().Split('\n');
            var lines = stackTraceList.Length;
            for (var i = 1; i < lines / 3; i++)
            {
                Assert.AreEqual(stackTraceList[i], stackTraceList[i + lines / 3]);
                Assert.AreEqual(stackTraceList[i], stackTraceList[i + lines / 3 * 2]);
            }
        }

        [Test]
        public void TestWritingExMsgNotThrowingInExecuteDataBase()
        {
            DwgCommandHelperActive.ExecuteDataBaseActions(db => throw ExInitInBase);
            MsgProviderMockInitInBase.Verify(m => m.Error(ExInitInBase), Times.Once);
        }
    }
}