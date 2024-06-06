using System.IO;
using System.Threading;
using ACADBase;
using CommonUtils.CustomExceptions;
using Moq;
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
            // PropDwgCommandHelperOfTestDwg.ExecuteDatabaseFuncs(AddLine, CheckLine);
            // Assert.True(PropDwgCommandHelperOfTestAddingLinesDwg.TestAddingLine());
            var result = DwgCommandHelperBase.ExecuteCustomCommands<DwgCommandHelperOfTestAddingLines>(TestDrawingPath,GetMsgProviderMockObj());
#if ApplicationTest
            if (!HostApplicationServiceWrapper.IsTargetDrawingActive(TestDrawingPath))
            {
                Assert.AreEqual(ArgumentExceptionOfInvalidDwgFile.CustomeMessage(TestDrawingPath),
                    result.CancelMessage);
                return;
            }

#endif
            Assert.True(result.IsSuccess);
        }

        [Test]
        public void TestAddLineInActiveDwg()
        {
            // Run the tests
            // PropDwgCommandHelperActive.ExecuteDatabaseFuncs(AddLine, CheckLine);
            Assert.True(DwgCommandHelperBase.ExecuteCustomCommands<DwgCommandHelperOfTestAddingLines>("", GetMsgProviderMockObj()).IsSuccess);
        }

        [Test]
        public void TestWrongDwgName()
        {
            var invaildPath = @"D:\NonExisting.dwg";
            Assert.False(File.Exists(invaildPath));
            Assert.AreEqual(DwgFileNotFoundException.CustomeMessage(invaildPath),
                DwgCommandHelperBase.ExecuteCustomCommands<DwgCommandHelperOfTestAddingLines>(invaildPath, GetMsgProviderMockObj())
                    .CancelMessage);
            Assert.True(File.Exists(TestTxtPath));
            Assert.AreEqual(DwgFileNotFoundException.CustomeMessage(TestTxtPath),
                DwgCommandHelperBase.ExecuteCustomCommands<DwgCommandHelperOfTestAddingLines>(TestTxtPath, GetMsgProviderMockObj())
                    .CancelMessage);
        }
        [Test]
        public void TestInvalidDwg()
        {
            Assert.True(File.Exists(FakeDrawingPath));
            Assert.AreEqual(NullReferenceExceptionOfDatabase.CustomeMessage(FakeDrawingPath),
                DwgCommandHelperBase.ExecuteCustomCommands<DwgCommandHelperOfTestAddingLines>(FakeDrawingPath, GetMsgProviderMockObj())
                    .CancelMessage);
        }
        
        // private void ExampleOfVerifyOnlyWorkOnce()
        // {
        //     Assert.Throws<MockException>(MsgProviderShowExInitInBaseOnce);
        // }
        //
        // private void ExampleShowsVerifyCheckingExactlySameObject()
        // {
        //     //Example shows parameter verify should be exactly the same object.
        //     Assert.Throws<MockException>(() =>
        //         MsgProviderVerifyExOnce(m => m.Error(new Exception())));
        // }
        //
        // [Test]
        // public void TestWritingExMsgWMsgBox()
        // {
        //     //THIS DO NOT WORK:Console.WriteLine("\nConsole WriteLine");
        //     DwgCommandHelperOfMsgBox.WriteMessage("Testing Msgbox AsProvider");
        //     //DwgCommandHelperOfMsgBox.WriteMessage($"WorkingDatabase:{HostApplicationServices.WorkingDatabase.Filename}");
        // }
        // //TODO: FIX THIS TEST
        // [Test]
        // public void TestExceptionScopeAndTrack()
        // {
        //     DwgCommandHelperOfRecordingExScopeAndTrack.ExecuteDatabaseFuncs(db => throw ExInitInBase);
        //     LogManager.GetCurrentClassLogger().Info(ExScopeStackTrace.ToString());
        //     TestVerifyExceptionReflectingLastMethod(ExScopeStackTrace);
        // }
        // //TODO: FIX THIS TEST
        // private static void TestVerifyExceptionReflectingLastMethod(StringBuilder exScopeStackTrace)
        // {
        //     Assert.True(exScopeStackTrace.ToString().Contains(nameof(TestExceptionScopeAndTrack)));
        // }
        //
        // [Test]
        // public void TestExceptionShouldNotBeThrownInDatabaseFunction()
        // {
        //     //Assert.Throws<TestException>(()=>
        //     PropDwgCommandHelperActive.ExecuteDatabaseFuncs(db => throw ExInitInBase);//);
        //     ExampleShowsVerifyCheckingExactlySameObject();
        //     MsgProviderShowExInitInBaseOnce();
        //     ExampleOfVerifyOnlyWorkOnce();
        // }
    }
}