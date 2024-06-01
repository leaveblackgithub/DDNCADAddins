using CommonUtils.StringLibs;
using NUnit.Framework;

namespace CommonUtils.Tests.LibsOfString
{
    [TestFixture]
    public class BoundPrefixUtilsTest
    {
        public const string T1B1FNjAbT1SkinTypAWallCurtainWall = "T1 B1-7F$0$NJ-AB_T1 Skin-typ$0$A-WALL-CURTAIN WALL";
        public const string AWallCurtainWall = "A-WALL-CURTAIN WALL";
        public const string testabc = "$$abc";

        [Test]
        public void HasBoundPrefixTest()
        {
            Assert.True(BoundPrefixUtils.HasBoundPrefix(T1B1FNjAbT1SkinTypAWallCurtainWall));
            Assert.False(BoundPrefixUtils.HasBoundPrefix(testabc));
        }

        [Test]
        public void RemoveBoundPrefixTest()
        {
            Assert.AreEqual(BoundPrefixUtils.RemoveBoundPrefix(T1B1FNjAbT1SkinTypAWallCurtainWall),
                AWallCurtainWall);
            Assert.AreEqual(BoundPrefixUtils.RemoveBoundPrefix(testabc), testabc);
        }
    }
}