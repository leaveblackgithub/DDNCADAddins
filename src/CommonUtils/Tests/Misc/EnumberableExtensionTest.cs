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

        // //Test for RunForEach
        // [Test]
        // public void RunForEachTest()
        // {
        //     var counter = new TestCounter();
        //     var result = _testFuncs._.RunForEach(counter);
        //     Assert.AreEqual(2, counter.Count);
        //     Assert.True(result.IsCancel);
        //     Assert.AreEqual(TestFuncs.TestExceptionForCancel, result.ExceptionInfo.SourceException);
        // }

        //
        // [Test]
        // public void RunForOnceAndMessageProviderMockUtilsTest()
        // {
        //     var counter = new TestCounter();
        //     RunForOnceAndMessageProviderMockUtilsTestMethod(TestFuncs.SucessMethod, counter, 1, false,
        //         nameof(TestFuncs.SucessMethod));
        //     var exceptionMessage = TestFuncs.TestExceptionForCancel.Message;
        //     RunForOnceAndMessageProviderMockUtilsTestMethod(TestFuncs.CancelMethod, counter, 2, true,
        //         exceptionMessage,m=>m.Error(It.IsAny<ExceptionDispatchInfo>()));
        //     RunForOnceAndMessageProviderMockUtilsTestMethod(c=>throw TestFuncs.TestExceptionForCancel, counter, 2, true,
        //         exceptionMessage, m => m.Error(It.IsAny<TestException>()));
        // }
        //
        // private void RunForOnceAndMessageProviderMockUtilsTestMethod(Func<TestCounter, FuncResult> func,
        //     TestCounter counter, int expectedCount, bool isCancel,string message, Expression<Action<IMessageProvider>> checkExceptAction=null)
        // {
        //     var result = EnumerableExtension.RunForOnce(func, counter, MessageProviderMockUtils.NewMessageProviderInstance(),EnumerableExtension.ShowSuccessFuncName.Show);
        //     Assert.AreEqual(expectedCount, counter.Count);
        //     Assert.AreEqual(isCancel, result.IsCancel);
        //     if(isCancel&&checkExceptAction!=null) MessageProviderMockUtils.MsgProviderVerifyExOnce(checkExceptAction);
        //     Assert.AreEqual(message, MessageProviderMockUtils.LastMessage);
        // }
    }
}