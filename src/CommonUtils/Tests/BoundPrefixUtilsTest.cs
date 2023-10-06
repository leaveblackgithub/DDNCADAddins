using NUnit.Framework;

namespace CommonUtils.Tests
{
    [TestFixture]
    public class BoundPrefixUtilsTest
    {
        [Test]
        public void HasBoundPrefixTest()
        {
            Assert.True(BoundPrefixUtils.HasBoundPrefix("T1 B1-7F$0$NJ-AB_T1 Skin-typ$0$A-WALL-CURTAIN WALL"));
            Assert.False(BoundPrefixUtils.HasBoundPrefix("$$abc"));
        }

        [Test]
        public void RemoveBoundPrefixTest()
        {
            Assert.AreEqual(BoundPrefixUtils.RemoveBoundPrefix("T1 B1-7F$0$NJ-AB_T1 Skin-typ$0$A-WALL-CURTAIN WALL"),
                "A-WALL-CURTAIN WALL");
            Assert.AreEqual(BoundPrefixUtils.RemoveBoundPrefix("$$abc"), "$$abc");
        }
    }
}