using CommonUtils.StringLibs;
using NUnit.Framework;

namespace CommonUtils.Tests.LibsOfString
{
    [TestFixture]
    public class StringUtilsTests
    {
        //Create a test method for EqualsIgnoreCase
        [Test]
        public void EqualsIgnoreCaseTest()
        {
            //Arrange
            var str1 = "Hello";
            var str2 = "hello";
            //Act
            var result = str1.EqualsIgnoreCase(str2);
            //Assert
            Assert.IsTrue(result);
            //Assert false
            Assert.IsFalse(str1.EqualsIgnoreCase("world"));
        }

        //Create a test method for StartsWithIgnoreCase
        [Test]
        public void StartsWithIgnoreCaseTest()
        {
            //Arrange
            var str1 = "Hello";


            var str2 = "hello world";
            //Act

            //Assert
            Assert.IsTrue(str2.StartsWithIgnoreCase(str1));
            Assert.IsTrue(str2.StartsWithIgnoreCase(str1, " W"));

            Assert.IsFalse(str2.StartsWithIgnoreCase(str1, ","));
        }

        //Create a test method for SplitByString
        [Test]
        public void SplitByStringTest()
        {
            //Arrange
            var str3 = "Hello,world,,";

            //Act

            //Assert
            Assert.AreEqual(4, str3.SplitByString(",").Length);
            Assert.AreEqual(3, str3.SplitByString("o").Length);
            Assert.AreEqual(2, str3.SplitByString("ll").Length);
            Assert.AreEqual(1, str3.SplitByString("LL").Length);
        }
    }
}