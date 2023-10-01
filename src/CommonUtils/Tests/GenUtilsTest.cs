using General;
using NUnit.Framework;

namespace CommonUtils.Tests
{
    [TestFixture]
    public class GenUtilsTest
    {
        [Test]
        public void HasBoundPrefixTest()
        {
            Assert.True(GenUtils.HasBoundPrefix("T1 B1-7F$0$NJ-AB_T1 Skin-typ$0$A-WALL-CURTAIN WALL"));
            Assert.False(GenUtils.HasBoundPrefix("$$abc"));
        }

        [Test]
        public void RemoveBoundPrefixTest()
        {
            Assert.AreEqual(GenUtils.RemoveBoundPrefix("T1 B1-7F$0$NJ-AB_T1 Skin-typ$0$A-WALL-CURTAIN WALL"),
                "A-WALL-CURTAIN WALL");
            Assert.AreEqual(GenUtils.RemoveBoundPrefix("$$abc"), "$$abc");
        }
    }
}