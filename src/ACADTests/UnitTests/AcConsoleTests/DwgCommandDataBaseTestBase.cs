using System;
using ACADBase;
using Autodesk.AutoCAD.DatabaseServices;
using Moq;
using NLog;
using NUnit.Framework;

namespace ACADTests.UnitTests.AcConsoleTests
{
    public class DwgCommandDataBaseTestBase
    {
        protected const string TestDrawingPath = @"D:\leaveblackgithub\DDNCADAddinsForRevitImport\src\ACADTests\TestDrawing.dwg";
        protected IDwgCommandHelper DwgCommandHelperTest;
        protected IDwgCommandHelper DwgCommandHelperActive;
        protected Mock<IMessageProvider> _mockMessageProvider;
        protected Exception _exception;
        protected Action<Database> emptyDbAction;

        [SetUp]
        public virtual void SetUp()
        {
            _mockMessageProvider = new Mock<IMessageProvider>();
            var messageProvider = _mockMessageProvider.Object;
            DwgCommandHelperTest = new DwgCommandHelper(
                TestDrawingPath, messageProvider);
            DwgCommandHelperActive = new DwgCommandHelper("", messageProvider);
            emptyDbAction = (db => LogManager.GetCurrentClassLogger().Info("EmptyDbAction"));
            _exception = new Exception("Test Exception in Execution");
        }
    }
}