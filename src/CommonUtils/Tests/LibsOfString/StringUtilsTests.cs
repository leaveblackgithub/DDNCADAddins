using NUnit.Framework;
using CommonUtils.LibsOfString;

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
            string str1 = "Hello";
            string str2 = "hello";
            //Act
            bool result = StringUtils.EqualsIgnoreCase(str1, str2);
            //Assert
            Assert.IsTrue(result);
            //Assert false
            Assert.IsFalse(StringUtils.EqualsIgnoreCase(str1, "world"));
        }

        //Create a test method for StartsWithIgnoreCase
        [Test]
        public void StartsWithIgnoreCaseTest()
        {
            //Arrange
            string str1 = "Hello";


            string str2 = "hello world";
            //Act

            //Assert
            Assert.IsTrue(StringUtils.StartsWithIgnoreCase(str2, str1));
            Assert.IsTrue(StringUtils.StartsWithIgnoreCase(str2, str1, " W"));

            Assert.IsFalse(StringUtils.StartsWithIgnoreCase(str2, str1,","));
        }
        //Create a test method for SplitByString
        [Test]
        public void SplitByStringTest()
        {
            //Arrange
            string str3 = "Hello,world,,";

            //Act

            //Assert
            Assert.AreEqual(4, StringUtils.SplitByString(str3, ",").Length);
            Assert.AreEqual(3, StringUtils.SplitByString(str3, "o").Length);
            Assert.AreEqual(2, StringUtils.SplitByString(str3, "ll").Length);
            Assert.AreEqual(1, StringUtils.SplitByString(str3, "LL").Length);
        }

    }
}