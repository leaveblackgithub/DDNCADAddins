using System.IO;
using System.Threading;
using ACADBase;
using CommonUtils.Misc;
using NUnit.Framework;

namespace ACADTestsOfUnit.UnitTests.AcConsoleTests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class DwgCommandHelperTests : DwgCommandHelperTestBase
    {
        public string TestAddingLinesName=nameof(TestDwgCommandHelper.TestAddingLines);
        //Use a new drawing

#if ApplicationTest
        [Test]
        public void TestAddLineInTestDwgOnApplication()
        {
            var result =
                BaseDwgCommandHelper.ExecuteCustomCommands<TestDwgCommandHelper>(TestDrawingPath,
                    TestAddingLinesName);
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
                BaseDwgCommandHelper
                    .ExecuteCustomCommands<TestDwgCommandHelper>(FakeDrawingPath)
                    .ErrorMessage);
        }

#endif
#if AcConsoleTest
        [Test]
        public void TestAddLineInTestDwgOnAcConsole()
        {
            var result =
                BaseDwgCommandHelper.ExecuteCustomCommands<TestDwgCommandHelper>(TestDrawingPath, TestAddingLinesName);
            Assert.True(result.IsSuccess);
        }

        [Test]
        public void TestInvalidDwgOnAcConcole()
        {
            Assert.True(File.Exists(FakeDrawingPath));
            Assert.AreEqual(ExceptionMessage.NullDatabase(FakeDrawingPath),
                BaseDwgCommandHelper
                    .ExecuteCustomCommands<TestDwgCommandHelper>(FakeDrawingPath)
                    .ErrorMessage);
        }

#endif

        [Test]
        public void TestAddLineInActiveDwg()
        {
            Assert.True(BaseDwgCommandHelper
                .ExecuteCustomCommands<TestDwgCommandHelper>("").IsSuccess);
        }

        [Test]
        public void TestWrongDwgName()
        {
            var invaildPath = @"D:\NonExisting.dwg";
            Assert.False(File.Exists(invaildPath));
            Assert.AreEqual(ExceptionMessage.IsNotExistingOrNotDwg(invaildPath),
                BaseDwgCommandHelper
                    .ExecuteCustomCommands<TestDwgCommandHelper>(invaildPath)
                    .ErrorMessage);
            Assert.True(File.Exists(TestTxtPath));
            Assert.AreEqual(ExceptionMessage.IsNotExistingOrNotDwg(TestTxtPath),
                BaseDwgCommandHelper
                    .ExecuteCustomCommands<TestDwgCommandHelper>(TestTxtPath)
                    .ErrorMessage);
        }
    }
}