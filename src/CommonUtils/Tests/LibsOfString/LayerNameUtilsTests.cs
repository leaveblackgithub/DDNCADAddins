using NUnit.Framework;
using CommonUtils.LibsOfString;

namespace CommonUtils.Tests.LibsOfString
{
    [TestFixture]
    public class LayerNameUtilsTests
    {
        public const string Layer1Lower = "layer1";
        public const string Layer1Upper = "LAYER1";
        public const string Layer2WBoundPrefix = BoundPrefixUtilsTest.T1B1FNjAbT1SkinTypAWallCurtainWall;
        public const string Layer3WoBoundPrefix = BoundPrefixUtilsTest.AWallCurtainWall;
        public const string Ltype1Lower = "ltype1";
        public const string Ltype2Lower = "ltype2";
        public const string LayerDefpoints = LayerNameUtils.LayerDefpoints;

        //Add true and false tests for AddLtypePrefixAndToUpper
        [Test]
        public void AddLtypePrefixAndToUpperTest()
        {
            Assert.AreEqual("LTYPE1___LAYER1", LayerNameUtils.AddLtypePrefixAndUpper(Layer1Lower,Ltype1Lower));
            Assert.AreNotEqual("LTYPE1___LAYER1", LayerNameUtils.AddLtypePrefixAndUpper(Layer1Lower, Ltype2Lower));
            Assert.AreEqual("LTYPE2___LAYER1", LayerNameUtils.AddLtypePrefixAndUpper(Layer1Lower, Ltype2Lower));
            Assert.AreEqual($"LTYPE2___{Layer3WoBoundPrefix}", LayerNameUtils.AddLtypePrefixAndUpper(Layer2WBoundPrefix, Ltype2Lower));
            Assert.AreEqual($"LTYPE2___{LayerDefpoints}", LayerNameUtils.AddLtypePrefixAndUpper(LayerDefpoints, Ltype2Lower));
        }
        // Add true and false tests for HasCorrectLtypePrefix
        [Test]
        public void IsCorrectPatternTest()
        {
            Assert.False(LayerNameUtils.IsCorrectPattern(Layer1Lower,Ltype1Lower));
            Assert.True(LayerNameUtils.IsCorrectPattern("LTYPE1___LAYER1", Ltype1Lower));
            Assert.False(LayerNameUtils.IsCorrectPattern("LTYPE1___LAYER1", Ltype2Lower));
            Assert.False(LayerNameUtils.IsCorrectPattern($"LTYPE2___{Layer2WBoundPrefix}", Ltype2Lower));
            Assert.True(LayerNameUtils.IsCorrectPattern($"LTYPE2___{Layer3WoBoundPrefix}", Ltype2Lower));
            Assert.True(LayerNameUtils.IsCorrectPattern($"LTYPE2___{LayerDefpoints}", Ltype2Lower));
        }
        // Add true and false tests for GetUpperShortName
        [Test]
        public void GetUpperShortNameTest()
        {
            Assert.AreEqual(Layer1Upper, LayerNameUtils.GetUpperShortName("LTYPE1___LAYER1"));
            Assert.AreNotEqual(Layer1Lower , LayerNameUtils.GetUpperShortName("LTYPE1___LAYER1"));
            Assert.AreNotEqual(Layer1Upper, LayerNameUtils.GetUpperShortName("LTYPE1___LAYER2"));
            Assert.AreEqual(Layer3WoBoundPrefix, LayerNameUtils.GetUpperShortName($"LTYPE2___{Layer2WBoundPrefix}"));
            Assert.AreEqual(Layer3WoBoundPrefix, LayerNameUtils.GetUpperShortName($"LTYPE2___{Layer3WoBoundPrefix}"));
            Assert.AreEqual(LayerDefpoints, LayerNameUtils.GetUpperShortName($"LTYPE2___{LayerDefpoints}"));
            Assert.AreEqual(LayerDefpoints, LayerNameUtils.GetUpperShortName(LayerDefpoints));
        }




    }
}