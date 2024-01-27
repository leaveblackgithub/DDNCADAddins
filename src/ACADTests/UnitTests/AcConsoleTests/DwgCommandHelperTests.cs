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
        //Use a new drawing

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
            
            DwgCommandHelperOfRecordingExScopeAndTrack.ExecuteDataBaseActions(db => throw ExInitInBase);
            LogManager.GetCurrentClassLogger().Info(ExScopeStackTrace.ToString());
            ExceptionsOfDifScopeHasSameStackTraceFrLine2(ExScopeStackTrace);
        }

        private static void ExceptionsOfDifScopeHasSameStackTraceFrLine2(StringBuilder exScopeStackTrace)
        {
            Assert.True(exScopeStackTrace.ToString().Contains(nameof(TestExceptionScopeAndTrack)));
        }

        [Test]
        public void TestWritingExMsgNotThrowingInExecuteDataBase()
        {
            DwgCommandHelperActive.ExecuteDataBaseActions(db => throw ExInitInBase);
            MsgProviderMockInitInBase.Verify(m => m.Error(ExInitInBase), Times.Once);
        }
    }
}