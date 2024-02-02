using System;
using System.Text;
using System.Threading;
using ACADBase;
using CommonUtils.CustomExceptions;
using Moq;
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
            DwgCommandHelperOfTestDwg.ExecuteDatabaseFuncs(db => AddLine(db), db => CheckLine(db));
        }

        [Test]
        public void TestAddLineInActiveDwg()
        {
            // Run the tests
            DwgCommandHelperActive.ExecuteDatabaseFuncs(db => AddLine(db), db => CheckLine(db));
        }

        [Test]
        public void TestWrongDwgName()
        {
            Assert.Throws<DwgFileNotFoundException>(() => new DwgCommandHelperOfAcConsole(
                @"D:\NonExisting.dwg"));
        }

        private void ExampleOfVerifyOnlyWorkOnce()
        {
            Assert.Throws<MockException>(MsgProviderShowExInitInBaseOnce);
        }

        private void ExampleShowsVerifyCheckingExactlySameObject()
        {
            //Example shows parameter verify should be exactly the same object.
            Assert.Throws<MockException>(() =>
                MsgProviderVerifyExOnce(m => m.Error(new Exception())));
        }

        [Test]
        public void TestWritingExMsgWMsgBox()
        {
            DwgCommandHelperOfMsgBox.WriteMessage("Testing MsgboxAsProvider");
        }

        [Test]
        public void TestExceptionScopeAndTrack()
        {
            DwgCommandHelperOfRecordingExScopeAndTrack.ExecuteDatabaseFuncs(db => throw ExInitInBase);
            LogManager.GetCurrentClassLogger().Info(ExScopeStackTrace.ToString());
            TestVerifyExceptionReflectingLastMethod(ExScopeStackTrace);
        }

        private static void TestVerifyExceptionReflectingLastMethod(StringBuilder exScopeStackTrace)
        {
            Assert.True(exScopeStackTrace.ToString().Contains(nameof(TestExceptionScopeAndTrack)));
        }

        [Test]
        public void TestWritingExMsgNotThrowingInExecuteDataBase()
        {
            DwgCommandHelperActive.ExecuteDatabaseFuncs(db => throw ExInitInBase);
            ExampleShowsVerifyCheckingExactlySameObject();
            MsgProviderShowExInitInBaseOnce();
            ExampleOfVerifyOnlyWorkOnce();
        }
    }
}