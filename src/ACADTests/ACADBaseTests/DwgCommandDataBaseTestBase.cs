using System;
using ACADBase;
using Autodesk.AutoCAD.DatabaseServices;
using Moq;
using NLog;
using NUnit.Framework;

namespace ACADTests.ACADBaseTests
{
    public class DwgCommandDataBaseTestBase
    {
        protected IDwgCommandBase _dwgCommandBaseTest;
        protected IDwgCommandBase _dwgCommandBaseActive;
        protected Mock<IMessageProvider> _mockMessageProvider;
        protected Exception _exception;
        protected Action<Database> emptyDbAction;

        [SetUp]
        public virtual void SetUp()
        {
            _mockMessageProvider = new Mock<IMessageProvider>();
            var messageProvider = _mockMessageProvider.Object;
            _dwgCommandBaseTest = new DwgCommandBase(
                @"D:\leaveblackgithub\DDNCADAddinsForRevitImport\src\ACADTests\TestDrawing.dwg", messageProvider);
            _dwgCommandBaseActive = new DwgCommandBase("", messageProvider);
            emptyDbAction = (db => LogManager.GetCurrentClassLogger().Info("EmptyDbAction"));
            _exception = new Exception("Test Exception in Execution");
        }
    }
}