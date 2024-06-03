using System;
using System.Linq.Expressions;
using ACADBase;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.Misc;
using CommonUtils.UtilsForTest;
using Moq;
using NLog;
using NUnit.Framework;

namespace ACADTests.UnitTests.AcConsoleTests
{
    //TODO CREATE SUB CLASS FOR TEST OF DWGCOMMANDHELPER
    public class DwgCommandHelperTestBase
    {
        protected const string TestDrawingName = "TestDrawing.dwg";
        protected const string TestTxtName = "TestTxt.txt";
        protected const string FakeDrawingName = "FakeDrawing.dwg";

        protected const string TestFolder = @"D:\leaveblackgithub\DDNCADAddinsForRevitImport\src\ACADTests\";
        
        private TestException _exInitInBase;


        protected string TestDrawingPath => TestFolder + TestDrawingName;
        protected string TestTxtPath => TestFolder + TestTxtName;
        protected string FakeDrawingPath => TestFolder + FakeDrawingName;

        protected TestException ExInitInBase =>
            _exInitInBase ?? (_exInitInBase = new TestException(nameof(ExInitInBase)));
        

        [SetUp]
        public virtual void SetUp()
        {
            MsgProviderInvokeClear();
        }

        [TearDown]
        public virtual void TearDown()
        {
            // MsgProviderInvokeClear();
        }

        protected IMessageProvider GetMsgProviderMockObj()
        {
            return MessageProviderMockUtils.NewMessageProviderInstance();
            // MsgProviderInvokeClear();
            // return MsgProviderMockInitInBase.Object;
        }

        private void MsgProviderInvokeClear()
        {
            MessageProviderMockUtils.MsgProviderInvokeClear();
            // MsgProviderMockInitInBase.Invocations.Clear();
        }

        protected void EmptyDbAction(Database db)
        {
            LogManager.GetCurrentClassLogger().Info("EmptyDbAction");
        }

        protected void MsgProviderShowExInitInBaseOnce()
        {
            MsgProviderVerifyExOnce(m => m.Error(ExInitInBase));
        }

        protected void MsgProviderVerifyExTypeOnce<T>() where T : Exception
        {
            MsgProviderVerifyExOnce(m => m.Error(It.IsAny<T>()));
        }

        protected void MsgProviderVerifyExOnce(Expression<Action<IMessageProvider>> checkExceptAction)
        {
            MessageProviderMockUtils.MsgProviderVerifyExOnce(checkExceptAction);
            // MsgProviderMockInitInBase.Verify(checkExceptAction, Times.Once);
            // MsgProviderInvokeClear();
        }
        
    }
}