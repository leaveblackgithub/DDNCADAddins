using System;
using System.Reflection;
using NLog;
using NUnit.Framework;

namespace CommonUtils.Tests
{
    [TestFixture]
    public class ReflectionExtensionTest
    {
        private static TestClass _testClass = new TestClass();
        private string _realproperty = "RealProperty";
        private string _fakeproperty = "FakeProperty";

        [SetUp]
        public void SetUp()
        {
            _testClass = new TestClass();
        }

        [Test]
        public void TryGetPropertyTest()
        {
            PropertyInfo property;
            Assert.True(_testClass.TryGetProperty<bool>(_realproperty, out property));
            Assert.NotNull(property);
            Assert.False(_testClass.TryGetProperty<string>(_realproperty, out property));
            Assert.Null(property);
            Assert.False(_testClass.TryGetProperty<bool>(_fakeproperty, out property));
            Assert.Null(property);
        }
        [Test]
        public void MustGetPropertyTest()
        {
            Assert.NotNull(_testClass.MustGetProperty<bool>(_realproperty));
            Assert.False(_testClass.GetObjectPropertyValue<bool>(_realproperty));
            Exception ex;
            ex = Assert.Throws<ArgumentException>(MustGetRealPropertyInString);
            Assert.That(ex.Message, Is.EqualTo("Type TestClass doesn't contain property RealProperty of type String"));
            ex = Assert.Throws<ArgumentException>(MustGetFakeProperty);
            Assert.That(ex.Message, Is.EqualTo("Type TestClass doesn't contain property FakeProperty of type Boolean"));
        }
        private void MustGetRealPropertyInString()
        {
            _testClass.MustGetProperty<string>(_realproperty);
        }
        private void MustGetFakeProperty()
        {
            _testClass.MustGetProperty<bool>(_fakeproperty);
        }
    }

    public class TestClass
    {
        public bool RealProperty { get; }
        public TestClass(bool realProperty = false)
        {
            RealProperty = realProperty;
        }

    }
}