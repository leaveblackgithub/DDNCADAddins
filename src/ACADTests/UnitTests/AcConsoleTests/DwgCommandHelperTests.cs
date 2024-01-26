using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ACADBase;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Moq;
using Moq.Protected;
using NLog;
using NUnit.Framework;
using Exception = System.Exception;

namespace ACADTests.UnitTests.AcConsoleTests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class DwgCommandHelperTests : DwgCommandHelperTestBase
    {
        private long _lineId;
        private TestException _exInitInSetup;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _lineId = 0;

            _exInitInSetup = new TestException(nameof(_exInitInSetup));
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
            DwgCommandHelperTest.ExecuteDataBaseActions(AddLine, CheckLine);
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
            var dwgCommandBaseMockProtected = new Mock<DwgCommandHelper>("", MsgProviderMockInitInSetup.Object);
            dwgCommandBaseMockProtected.Protected().Setup("RunCommandActions").Throws(ExInitInBase).Verifiable(Times.Once);
            dwgCommandBaseMockProtected.Object.ExecuteDataBaseActions(EmptyDbAction);
            //Example shows parameter verify should be exactly the same object.
            Assert.Throws<MockException>(() => MsgProviderMockInitInSetup.Verify(m => m.Error(new Exception()), Times.Once));
            MsgProviderMockInitInSetup.Verify(m => m.Error(ExInitInBase), Times.Once);
            MsgProviderMockInitInSetup.Verify(m => m.Error(ExInitInBase), Times.Once);
            MsgProviderMockInitInSetup.Invocations.Clear();
            Assert.Throws<MockException>(() => MsgProviderMockInitInSetup.Verify(m => m.Error(ExInitInBase), Times.Once));
        }

        [Test]
        public void TestWritingExMsgWMsgBox()
        {
            var dwgCommandHelperOfMsgBox = new DwgCommandHelper("");
            dwgCommandHelperOfMsgBox.WriteMessage("Testing MsgboxAsProvider");
        }



        [Test]
        public void TestExceptionScopeAndTrack()
        {
            var msgProviderMock=new Mock<IMessageProvider>();
            StringBuilder exscopeandtrack=new StringBuilder();
            msgProviderMock.Setup(m => m.Error(It.IsAny<Exception>()))
                .Callback<Exception>(e => exscopeandtrack.AppendLine($"[{e.Message}]:{e.StackTrace}"));
            var dwgCommandHelperOfRecordingExScopeAndTrack = new DwgCommandHelper("",msgProviderMock.Object);
            var exInitInMethod = new TestException("exInitInmethod");
            dwgCommandHelperOfRecordingExScopeAndTrack.ExecuteDataBaseActions(db => throw exInitInMethod);
            dwgCommandHelperOfRecordingExScopeAndTrack.ExecuteDataBaseActions(db => throw _exInitInSetup);
            dwgCommandHelperOfRecordingExScopeAndTrack.ExecuteDataBaseActions(db => throw ExInitInBase);
            LogManager.GetCurrentClassLogger().Info(exscopeandtrack.ToString());
            ExceptionsOfDifScopeHasSameStackTraceFrLine2(exscopeandtrack);
        }

        private static void ExceptionsOfDifScopeHasSameStackTraceFrLine2(StringBuilder exscopeandtrack)
        {
            string[] stackTraceList = exscopeandtrack.ToString().Split('\n');
            int lines = stackTraceList.Length;
            for (int i = 1; i < lines / 3; i++)
            {
                Assert.AreEqual(stackTraceList[i], stackTraceList[i + lines / 3]);
                Assert.AreEqual(stackTraceList[i], stackTraceList[i + lines / 3 * 2]);
            }
        }

        [Test]
        public void TestWritingExMsgNotThrowingInExecuteDataBase()
        {
            DwgCommandHelperActive.ExecuteDataBaseActions(db => throw ExInitInBase);
            MsgProviderMockInitInSetup.Verify(m => m.Error(ExInitInBase), Times.Once);
        }
        
    }
}