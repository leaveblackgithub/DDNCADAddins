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
    }
}