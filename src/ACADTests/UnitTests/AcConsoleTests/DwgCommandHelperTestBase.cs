using System;
using System.Linq.Expressions;
using ACADBase;
using Autodesk.AutoCAD.DatabaseServices;
using Moq;
using NLog;
using NUnit.Framework;

namespace ACADTests.UnitTests.AcConsoleTests
{
    public class DwgCommandHelperTestBase
    {
        protected const string TestDrawingPath = @"D:\leaveblackgithub\DDNCADAddinsForRevitImport\src\ACADTests\TestDrawing.dwg";
        protected IDwgCommandHelper DwgCommandHelperTest;
        protected IDwgCommandHelper DwgCommandHelperActive;
        protected Mock<IMessageProvider> MsgProviderMockInitInBase =>_msgProviderMockInitInSetup??(_msgProviderMockInitInSetup=new Mock<IMessageProvider>());
        // protected Action<Database> EmptyDbAction;
        private TestException _exInitInBase;
        private Mock<IMessageProvider> _msgProviderMockInitInSetup;

        protected TestException ExInitInBase=>_exInitInBase??(_exInitInBase=new TestException(nameof(ExInitInBase)));

        [SetUp]
        public virtual void SetUp()
        {
            var messageProvider = MsgProviderMockInitInBase.Object;
            DwgCommandHelperTest = new DwgCommandHelper(
                TestDrawingPath, messageProvider);
            DwgCommandHelperActive = new DwgCommandHelper("", messageProvider);
            // EmptyDbAction = (db => LogManager.GetCurrentClassLogger().Info("EmptyDbAction"));
        }

        [TearDown]
        public virtual void TearDown()
        {
            MsgProviderMockInitInBase.Invocations.Clear();
        }
        protected void EmptyDbAction(Database db)
        {
            LogManager.GetCurrentClassLogger().Info("EmptyDbAction");
        }

        protected void MsgProviderShowExInitInBaseOnce()
        {
            MsgProviderVerifyExOnce(m=>m.Error(ExInitInBase));
        }

        protected void MsgProviderVerifyExTypeOnce<T>() where T : Exception
        {
            MsgProviderVerifyExOnce(m=>m.Error(It.IsAny<T>()));
        }
        protected void MsgProviderVerifyExOnce(Expression<Action<IMessageProvider>> checkExceptAction)
        {
            MsgProviderMockInitInBase.Verify(checkExceptAction, Times.Once);
            MsgProviderMockInitInBase.Invocations.Clear();
        }
    }
}