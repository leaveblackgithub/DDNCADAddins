using System.IO;
using System.Threading;
using ACADBase;
using CommonUtils.Misc;
using NUnit.Framework;

namespace ACADTests.UnitTests.AcConsoleTests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class DwgCommandHelperTests : DwgCommandHelperTestBase
    {
        //Use a new drawing

#if ApplicationTest
        [Test]
        public void TestAddLineInTestDwgOnApplication()
        {
            // Run the tests
            // PropDwgCommandHelperOfTestDwg.ExecuteDatabaseFuncs(AddLine, CheckLine);
            // Assert.True(PropDwgCommandHelperOfTestAddingLinesDwg.TestAddingLine());
            var result =
                DwgCommandHelperBase.ExecuteCustomCommands<DwgCommandHelperOfTest>(TestDrawingPath,
                     TestAddingLines);
            if (!HostApplicationServiceWrapper.IsTargetDrawingActive(TestDrawingPath).IsSuccess)
            {
                Assert.AreEqual(ExceptionMessage.NullActiveDocument(TestDrawingPath),
                    result.ErrorMessage);
                return;
            }
            Assert.True(result.IsSuccess);
        }
        [Test]
        public void TestInvalidDwgOnApplication()
        {
            Assert.True(File.Exists(FakeDrawingPath));
            Assert.AreEqual(ExceptionMessage.NullActiveDocument(FakeDrawingPath),
                DwgCommandHelperBase
                    .ExecuteCustomCommands<DwgCommandHelperOfTest>(FakeDrawingPath)
                    .ErrorMessage);
        }

#endif
#if AcConsoleTest
        [Test]
        public void TestAddLineInTestDwgOnAcConsole()
        {
            // Run the tests
            // PropDwgCommandHelperOfTestDwg.ExecuteDatabaseFuncs(AddLine, CheckLine);
            // Assert.True(PropDwgCommandHelperOfTestAddingLinesDwg.TestAddingLine());
            var result =
                DwgCommandHelperBase.ExecuteCustomCommands<DwgCommandHelperOfTest>(TestDrawingPath,TestAddingLines);
            Assert.True(result.IsSuccess);
        }
        [Test]
        public void TestInvalidDwgOnAcConcole()
        {
            Assert.True(File.Exists(FakeDrawingPath));
            Assert.AreEqual(ExceptionMessage.NullDatabase(FakeDrawingPath),
                DwgCommandHelperBase
                    .ExecuteCustomCommands<DwgCommandHelperOfTest>(FakeDrawingPath)
                    .ErrorMessage);
        }

#endif

        [Test]
        public void TestAddLineInActiveDwg()
        {
            // Run the tests
            // PropDwgCommandHelperActive.ExecuteDatabaseFuncs(AddLine, CheckLine);
            Assert.True(DwgCommandHelperBase
                .ExecuteCustomCommands<DwgCommandHelperOfTest>("").IsSuccess);
        }

        [Test]
        public void TestWrongDwgName()
        {
            var invaildPath = @"D:\NonExisting.dwg";
            Assert.False(File.Exists(invaildPath));
            Assert.AreEqual(ExceptionMessage.IsNotExistingOrNotDwg(invaildPath),
                DwgCommandHelperBase
                    .ExecuteCustomCommands<DwgCommandHelperOfTest>(invaildPath)
                    .ErrorMessage);
            Assert.True(File.Exists(TestTxtPath));
            Assert.AreEqual(ExceptionMessage.IsNotExistingOrNotDwg(TestTxtPath),
                DwgCommandHelperBase
                    .ExecuteCustomCommands<DwgCommandHelperOfTest>(TestTxtPath)
                    .ErrorMessage);
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