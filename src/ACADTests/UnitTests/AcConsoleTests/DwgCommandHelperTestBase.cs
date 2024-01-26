using System;
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
        protected Mock<IMessageProvider> MsgProviderMockInitInSetup;
        // protected Action<Database> EmptyDbAction;
        private TestException _exInitInBase;

        protected TestException ExInitInBase=>_exInitInBase??(_exInitInBase=new TestException(nameof(ExInitInBase)));

        [SetUp]
        public virtual void SetUp()
        {
            MsgProviderMockInitInSetup = new Mock<IMessageProvider>();//Property will cause error? Must be init for each test
            var messageProvider = MsgProviderMockInitInSetup.Object;
            DwgCommandHelperTest = new DwgCommandHelper(
                TestDrawingPath, messageProvider);
            DwgCommandHelperActive = new DwgCommandHelper("", messageProvider);
            // EmptyDbAction = (db => LogManager.GetCurrentClassLogger().Info("EmptyDbAction"));
        }

        protected void EmptyDbAction(Database db)
        {
            LogManager.GetCurrentClassLogger().Info("EmptyDbAction");
        }
    }
}