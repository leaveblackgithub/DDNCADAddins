using System;
using System.Linq;
using CommonUtils.CustomExceptions;
using CommonUtils.Misc;
using CommonUtils.UtilsForTest;
using NUnit.Framework;

namespace CommonUtils.Tests.Misc
{
    [TestFixture]
    public class EnumberableExtensionTest
    {
        private TestException _testExceptionForCancel;
        private Func<TestCounter, CommandResult>[] _testFuncs;

        private TestException TestExceptionForCancel => _testExceptionForCancel ??
                                                        (_testExceptionForCancel =
                                                            new TestException("Test Exception for Counter"));

        private Func<TestCounter, CommandResult>[] TestFuncs => _testFuncs ?? (_testFuncs =
            new Func<TestCounter, CommandResult>[]
            {
                SucessMethod,
                CancelMethod,
                SucessMethod,
                CancelMethod
            });
        //Test for IsNullOrEmpty
        [Test]
        public void IsNullOrEmptyTest()
        {
            Assert.True(((string[])null).IsNullOrEmpty());
            Assert.True(new string[0].IsNullOrEmpty());
            Assert.False(new[] { "1" }.IsNullOrEmpty());
        }

        //Test for RunForEach
        [Test]
        public void RunForEachTest()
        {
            var counter = new TestCounter();
            var result = TestFuncs.RunForEach(counter);
            Assert.AreEqual(2, counter.Count);
            Assert.True(result.IsCancel);
            Assert.AreEqual(TestExceptionForCancel, result.ExceptionInfo.SourceException);
        }
        //Test for TestForEach
        [Test]
        public void TestForEachTest()
        {
            var counter = new TestCounter();
            var results = TestFuncs.TestForEach(counter);
            Assert.AreEqual(4, counter.Count);
            Assert.AreEqual(4, results.Count);
            Assert.AreEqual(TestExceptionForCancel, results.ElementAt(3).Value.ExceptionInfo.SourceException);
        }
        public CommandResult SucessMethod(TestCounter counter)
        {
            counter.Increment();
            return new CommandResult();
        }

        public CommandResult CancelMethod(TestCounter counter)
        {
            counter.Increment();
            return new CommandResult().Cancel(TestExceptionForCancel);
        }
    }
}