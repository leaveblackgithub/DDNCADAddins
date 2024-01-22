using ACADBase;
using Autodesk.AutoCAD.DatabaseServices;
using Moq;
using NUnit.Framework;

namespace ACADTests.UnitTests.AcConsoleTests
{
    [TestFixture]
    public class DatabaseExtensionTest:DwgCommandDataBaseTestBase
    {
        [Test]
        public void RunFuncInTransactionExceptionTest()
        {
            DwgCommandHelperActive.ExecuteDataBaseActions(db => db.RunFuncInTransaction(tr =>throw _exception));
            _mockMessageProvider.Verify(m => m.Error(_exception),
                Times.Once);
        }
        [Test]
        public void CreateObjExceptionTest()
        {
            DwgCommandHelperActive.ExecuteDataBaseActions(db => db.CreateInModelSpace<Line>(l=>throw _exception));
            _mockMessageProvider.Verify(m => m.Error(_exception),
                Times.Once);
        }

        [Test]
        public void GetDwgNameTest()
        {
            string dwgName = "";
            DwgCommandHelperTest.ExecuteDataBaseActions(db => dwgName=db.GetDwgName());
            Assert.AreEqual(TestDrawingPath, dwgName);

            DwgCommandHelperActive.ExecuteDataBaseActions(db => dwgName = db.GetDwgName());
            Assert.AreEqual("C:\\Users\\CFDDN\\AppData\\Local\\Autodesk\\AutoCAD 2019\\R23.0\\enu\\Template\\acadiso.dwt", dwgName);
        }
    }
}