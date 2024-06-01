using System;
using System.Text;
using System.Threading;
using ACADBase;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.CustomExceptions;
using CommonUtils.UtilsForTest;
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
            DwgCommandHelperOfTestDwg.ExecuteDatabaseFuncs(AddLine, CheckLine);
        }

        [Test]
        public void TestAddLineInActiveDwg()
        {
            // Run the tests
            DwgCommandHelperActive.ExecuteDatabaseFuncs(AddLine, CheckLine);
        }

        [Test]
        public void TestWrongDwgName()
        {
            Assert.Throws<DwgFileNotFoundException>(() => new DwgCommandHelperBaseInAcadBase(
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
            //THIS DO NOT WORK:Console.WriteLine("\nConsole WriteLine");
            DwgCommandHelperOfMsgBox.WriteMessage("Testing Msgbox AsProvider");
            //DwgCommandHelperOfMsgBox.WriteMessage($"WorkingDatabase:{HostApplicationServices.WorkingDatabase.Filename}");
        }
        //TODO: FIX THIS TEST
        [Test]
        public void TestExceptionScopeAndTrack()
        {
            DwgCommandHelperOfRecordingExScopeAndTrack.ExecuteDatabaseFuncs(db => throw ExInitInBase);
            LogManager.GetCurrentClassLogger().Info(ExScopeStackTrace.ToString());
            TestVerifyExceptionReflectingLastMethod(ExScopeStackTrace);
        }
        //TODO: FIX THIS TEST
        private static void TestVerifyExceptionReflectingLastMethod(StringBuilder exScopeStackTrace)
        {
            Assert.True(exScopeStackTrace.ToString().Contains(nameof(TestExceptionScopeAndTrack)));
        }

        [Test]
        public void TestExceptionShouldNotBeThrownInDatabaseFunction()
        {
            //Assert.Throws<TestException>(()=>
            DwgCommandHelperActive.ExecuteDatabaseFuncs(db => throw ExInitInBase);//);
            ExampleShowsVerifyCheckingExactlySameObject();
            MsgProviderShowExInitInBaseOnce();
            ExampleOfVerifyOnlyWorkOnce();
        }
    }
}