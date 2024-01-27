﻿using System;
using System.Text;
using System.Threading;
using ACADBase;
using Moq;
using NLog;
using NUnit.Framework;

namespace ACADTests.UnitTests.AcConsoleTests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class DwgCommandHelperTests : DwgCommandHelperTestBase
    {
        //Use a new drawing

        [Test]
        public void TestAddLineInTestDwg()
        {
            // Run the tests
            DwgCommandHelperOfTestDwg.ExecuteDataBaseActions(AddLine, CheckLine);
        }

        [Test]
        public void TestAddLineInActiveDwg()
        {
            // Run the tests
            DwgCommandHelperActive.ExecuteDataBaseActions(AddLine, CheckLine);
        }

        [Test]
        public void TestWrongDwgName()
        {
            Assert.Throws<ArgumentException>(() => new DwgCommandHelper(
                @"D:\NonExisting.dwg"));
        }

        private void ExampleOfVerifyOnlyWorkOnce()
        {
            Assert.Throws<MockException>(MsgProviderShowExInitInBaseOnce);
        }

        private void ExampleShowsVerifyCheckingExactlySameObject()
        {
            //Example shows parameter verify should be exactly the same object.
            Assert.Throws<MockException>(() =>
                MsgProviderVerifyExOnce(m=>m.Error(new Exception())));
        }

        [Test]
        public void TestWritingExMsgWMsgBox()
        {
            DwgCommandHelperOfMsgBox.WriteMessage("Testing MsgboxAsProvider");
        }

        [Test]
        public void TestExceptionScopeAndTrack()
        {
            DwgCommandHelperOfRecordingExScopeAndTrack.ExecuteDataBaseActions(db => throw ExInitInBase);
            LogManager.GetCurrentClassLogger().Info(ExScopeStackTrace.ToString());
            TestVerifyExceptionReflectingLastMethod(ExScopeStackTrace);
        }

        private static void TestVerifyExceptionReflectingLastMethod(StringBuilder exScopeStackTrace)
        {
            Assert.True(exScopeStackTrace.ToString().Contains(nameof(TestExceptionScopeAndTrack)));
        }

        [Test]
        public void TestWritingExMsgNotThrowingInExecuteDataBase()
        {
            DwgCommandHelperActive.ExecuteDataBaseActions(db => throw ExInitInBase);
            ExampleShowsVerifyCheckingExactlySameObject();
            MsgProviderShowExInitInBaseOnce();
            ExampleOfVerifyOnlyWorkOnce();
        }
    }
}