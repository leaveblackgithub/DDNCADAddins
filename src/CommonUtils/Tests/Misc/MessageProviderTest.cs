
#if AcConsoleTest

using CommonUtils.Misc;
using NUnit.Framework;

namespace CommonUtils.Tests.Misc
{
    [TestFixture]
    public class MessageProviderTest
    {
        [Test]
        public void TestShow()
        {
            string message = "Hello World";
            MessageProvider._.Show(message);
            Assert.AreEqual(MessageRecent._,message);
        }
    }
}
#endif