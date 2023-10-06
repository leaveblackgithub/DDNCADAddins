using ACADBase;
using Autodesk.AutoCAD.DatabaseServices;
using Moq;
using NUnit.Framework;

namespace ACADTests.ACADBaseTests
{
    [TestFixture]
    public class DatabaseExtensionTest:DwgCommandDataBaseTestBase
    {
        [Test]
        public void RunFuncInTransactionExceptionTest()
        {
            _dwgCommandBaseActive.ExecuteDataBaseActions(db => db.RunFuncInTransaction(tr =>throw _exception));
            _mockMessageProvider.Verify(m => m.Error(_exception),
                Times.Once);
        }
        [Test]
        public void CreateObjExceptionTest()
        {
            _dwgCommandBaseActive.ExecuteDataBaseActions(db => db.CreateInModelSpace<Line>(l=>throw _exception));
            _mockMessageProvider.Verify(m => m.Error(_exception),
                Times.Once);
        }
    }
}