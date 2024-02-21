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
        private readonly TestFuncs _testFuncs = new TestFuncs();

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
            var result = _testFuncs._.RunForEach(counter);
            Assert.AreEqual(2, counter.Count);
            Assert.True(result.IsCancel);
            Assert.AreEqual(TestFuncs.TestExceptionForCancel, result.ExceptionInfo.SourceException);
        }
        //Test for TestForEach
        [Test]
        public void TestForEachTest()
        {
            var counter = new TestCounter();
            var results = _testFuncs._.TestForEach(counter);
            Assert.AreEqual(4, counter.Count);
            Assert.AreEqual(4, results.Count);
            Assert.AreEqual(TestFuncs.TestExceptionForCancel, results.ElementAt(3).Value.ExceptionInfo.SourceException);
        }

        //TODO: Test for RunForOnce
        

    }
}