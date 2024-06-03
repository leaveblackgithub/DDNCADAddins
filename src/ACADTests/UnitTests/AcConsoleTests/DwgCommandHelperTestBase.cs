using System;
using System.Linq.Expressions;
using System.Text;
using ACADBase;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.CustomExceptions;
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
        protected const string TestDrawingPath =
            @"D:\leaveblackgithub\DDNCADAddinsForRevitImport\src\ACADTests\TestDrawing.dwg";

        private Mock<DwgCommandHelperBase> _dwgCommandBaseMockProtected;
        private DwgCommandHelperBase _dwgCommandHelperOfMsgBox;
        private DwgCommandHelperBase _dwgCommandHelperOfRecordingExScopeAndTrack;

        // protected Action<Database> EmptyDbAction;
        private TestException _exInitInBase;
        private StringBuilder _exScopeStackTrace;
        private Mock<IMessageProvider> _msgProviderMockToRecordEx;
        

        protected DwgCommandHelperOfTestAddingLines PropDwgCommandHelperOfTestAddingLinesDwg { get; set; }

        protected DwgCommandHelperOfTestAddingLines PropDwgCommandHelperActive { get; set; }

        protected TestException ExInitInBase =>
            _exInitInBase ?? (_exInitInBase = new TestException(nameof(ExInitInBase)));

        protected DwgCommandHelperBase DwgCommandHelperOfMsgBox =>
            _dwgCommandHelperOfMsgBox ?? (_dwgCommandHelperOfMsgBox = new DwgCommandHelperBase());

        protected StringBuilder ExScopeStackTrace => _exScopeStackTrace ?? (_exScopeStackTrace = new StringBuilder());

        protected DwgCommandHelperBase DwgCommandHelperOfRecordingExScopeAndTrack =>
            _dwgCommandHelperOfRecordingExScopeAndTrack ?? (_dwgCommandHelperOfRecordingExScopeAndTrack =
                new DwgCommandHelperBase("", MsgProviderMockToRecordEx.Object));

        protected Mock<IMessageProvider> MsgProviderMockToRecordEx
        {
            get
            {
                if (_msgProviderMockToRecordEx == null)
                {
                    _msgProviderMockToRecordEx = new Mock<IMessageProvider>();
                    _msgProviderMockToRecordEx.Setup(m => m.Error(It.IsAny<Exception>()))
                        .Callback<Exception>(e => ExScopeStackTrace.AppendLine($"[{e.Message}]:{e.StackTrace}"));
                }

                return _msgProviderMockToRecordEx;
            }
        }

        protected HandleValue LineHandleValue { get; private set; }
        
        [SetUp]
        public virtual void SetUp()
        {
            // DwgCommandHelperTest = new DwgCommandHelper(
            //     TestDrawingPath, messageProvider);
            // DwgCommandHelperActive = new DwgCommandHelper("", messageProvider);
            // EmptyDbAction = (db => LogManager.GetCurrentClassLogger().Info("EmptyDbAction"));
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

        protected CommandResult AddLine(IDatabaseHelper db)
        {
            LineHandleValue = null;
            var result = db.CreateInCurrentSpace<Line>(out var resultHandleValue);

            LineHandleValue = resultHandleValue;
            return result;
        }

        protected CommandResult CheckLine(IDatabaseHelper db)
        {
            //Check in another transaction if the line was created

            if (!db.TryGetObjectId(LineHandleValue, out _)) Assert.Fail("Line didn't created");
            LineHandleValue = null;
            return new CommandResult();
        }
    }
}