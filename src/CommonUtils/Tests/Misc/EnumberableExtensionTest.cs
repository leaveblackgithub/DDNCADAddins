using System;
using CommonUtils.CustomExceptions;
using CommonUtils.Misc;
using NUnit.Framework;

namespace CommonUtils.Tests.Misc
{
    [TestFixture]
    public class EnumberableExtensionTest
    {
        private TestException _testExceptionForCancel;

        private TestException TestExceptionForCancel => _testExceptionForCancel ??
                                                        (_testExceptionForCancel =
                                                            new TestException("Test Exception for Counter"));

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
            var funcs = new Func<TestCounter, CommandResult>[]
            {
                SucessMethod,
                CancelMethod,
                SucessMethod,
                CancelMethod
            };
            var result = funcs.RunForEach(counter);
            Assert.AreEqual(2, counter.Count);
            Assert.True(result.IsCancel);
            Assert.AreEqual(TestExceptionForCancel, result.ExceptionInfo.SourceException);
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