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

        private Mock<DwgCommandHelperOfAcConsole> _dwgCommandBaseMockProtected;
        private DwgCommandHelperOfAcConsole _dwgCommandHelperOfMsgBox;
        private DwgCommandHelperOfAcConsole _dwgCommandHelperOfRecordingExScopeAndTrack;
        private IDwgCommandHelper _dwgCommandHelperOfTestDwg;

        private IDwgCommandHelper _dwgHelperActive;

        // protected Action<Database> EmptyDbAction;
        private TestException _exInitInBase;
        private StringBuilder _exScopeStackTrace;
        private Mock<IMessageProvider> _msgProviderMockInitInSetup;
        private Mock<IMessageProvider> _msgProviderMockToRecordEx;

        protected IDwgCommandHelper DwgCommandHelperOfTestDwg =>
            _dwgCommandHelperOfTestDwg ?? (_dwgCommandHelperOfTestDwg =
                new DwgCommandHelperOfAcConsole(TestDrawingPath)); //, GetMsgProviderMockObj()));

        protected IDwgCommandHelper DwgCommandHelperActive => _dwgHelperActive ??
                                                              (_dwgHelperActive =
                                                                  new DwgCommandHelperOfAcConsole("",
                                                                      GetMsgProviderMockObj()));

        protected Mock<IMessageProvider> MsgProviderMockInitInBase => _msgProviderMockInitInSetup ??
                                                                      (_msgProviderMockInitInSetup =
                                                                          new Mock<IMessageProvider>());

        protected TestException ExInitInBase =>
            _exInitInBase ?? (_exInitInBase = new TestException(nameof(ExInitInBase)));

        protected DwgCommandHelperOfAcConsole DwgCommandHelperOfMsgBox =>
            _dwgCommandHelperOfMsgBox ?? (_dwgCommandHelperOfMsgBox = new DwgCommandHelperOfAcConsole());

        protected Mock<DwgCommandHelperOfAcConsole> DwgCommandBaseMockProtected => _dwgCommandBaseMockProtected ??
            (_dwgCommandBaseMockProtected =
                new Mock<DwgCommandHelperOfAcConsole>("",
                    GetMsgProviderMockObj()));

        protected StringBuilder ExScopeStackTrace => _exScopeStackTrace ?? (_exScopeStackTrace = new StringBuilder());

        protected DwgCommandHelperOfAcConsole DwgCommandHelperOfRecordingExScopeAndTrack =>
            _dwgCommandHelperOfRecordingExScopeAndTrack ?? (_dwgCommandHelperOfRecordingExScopeAndTrack =
                new DwgCommandHelperOfAcConsole("", MsgProviderMockToRecordEx.Object));

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
            MsgProviderInvokeClear();
            return MsgProviderMockInitInBase.Object;
        }

        private void MsgProviderInvokeClear()
        {
            MsgProviderMockInitInBase.Invocations.Clear();
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
            MsgProviderMockInitInBase.Verify(checkExceptAction, Times.Once);
            MsgProviderInvokeClear();
        }

        protected CommandResult AddLine(IDatabaseHelper db)
        {
            LineHandleValue = null;
            var result = db.CreateInModelSpace<Line>(out var resultHandleValue);

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