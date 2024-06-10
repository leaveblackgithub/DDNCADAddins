using System;
using CommonUtils.Misc;
using CommonUtils.StringLibs;
using NUnit.Framework;

namespace CommonUtils.Tests.Misc
{
    [TestFixture]
    public class DateTimeUtilsTest
    {
        //Test for each methods in DateTimeUtils
        [Test]
        public void DateTimeToTimeStampTest()
        {
            var dateTime = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var timeStamp = DateTimeUtils.DateTimeToTimeStamp(dateTime);
            Assert.AreEqual(1577836800, timeStamp);
        }

        [Test]
        public void DateTimeToLongTimeStampTest()
        {
            var dateTime = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var timeStamp = DateTimeUtils.DateTimeToLongTimeStamp(dateTime);
            Assert.AreEqual(1577836800000, timeStamp);
        }


        private void AddTimeStampeTest(Func<string, string> getStampedContent,
            Func<string, string, bool> startsOrEndsWith, Func<string, int, string> getStampSubString)
        {
            var content = "Test";
            var stamp1 = DateTimeUtils.DateTimeToLongTimeStampOfNow();
            var stampedContent = getStampedContent(content);
            var stamp2 = DateTimeUtils.DateTimeToLongTimeStampOfNow();
            var contentLength = content.Length;
            var stampLength = stampedContent.Length;
            var spanExpected = stamp2 - stamp1;
            Assert.True(startsOrEndsWith(stampedContent, content));
            var stampOfContent = long.Parse(getStampSubString(stampedContent, stampLength - contentLength));
            var spanActual = stampOfContent - stamp1;
            Assert.GreaterOrEqual(spanExpected, spanActual);
            Assert.LessOrEqual(0, spanActual);
        }

        private bool StringStartsWith(string str1, string str2)
        {
            return str1.StartsWith(str2);
        }

        private bool StringEndsWith(string str1, string str2)
        {
            return str1.EndsWith(str2);
        }

        [Test]
        public void AddTimeStampSuffixTest()
        {
            AddTimeStampeTest(DateTimeUtils.AddTimeStampSuffix, StringStartsWith, StringUtils.SubStringRight);
        }

        [Test]
        public void AddTimeStampPrefixTest()
        {
            AddTimeStampeTest(DateTimeUtils.AddTimeStampPrefix, StringEndsWith, StringUtils.SubStringLeft);
        }
    }
}