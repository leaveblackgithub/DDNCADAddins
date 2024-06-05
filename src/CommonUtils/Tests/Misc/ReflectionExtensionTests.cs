using System;
using System.Reflection;
using CommonUtils.CustomExceptions;
using CommonUtils.Misc;
using NUnit.Framework;

namespace CommonUtils.Tests.Misc
{
    [TestFixture]
    public class ReflectionExtensionTest
    {
        [SetUp]
        public void SetUp()
        {
            _testClass = new TestClass();
        }

        private static TestClass _testClass = new TestClass();
        private readonly string _realproperty = "RealProperty";
        private readonly string _fakeproperty = "FakeProperty";

        [Test]
        public void TryGetPropertyTest()
        {
            PropertyInfo property;
            Assert.True(_testClass.TryGetPropertyOfSpecificType<bool>(_realproperty, out property));
            Assert.NotNull(property);
            Assert.False(_testClass.TryGetPropertyOfSpecificType<string>(_realproperty, out property));
            Assert.Null(property);
            Assert.False(_testClass.TryGetPropertyOfSpecificType<bool>(_fakeproperty, out property));
            Assert.Null(property);
        }

        [Test]
        public void MustGetPropertyTest()
        {
            Assert.NotNull(_testClass.MustGetProperty<bool>(_realproperty));
            Assert.False(_testClass.GetObjectPropertyValue<bool>(_realproperty));
            Exception ex;
            ex = Assert.Throws<ArgumentExceptionOfInvalidProperty>(MustGetRealPropertyInString);
            Assert.That(ex.Message, Is.EqualTo("Type TestClass doesn't contain property RealProperty of type String"));
            ex = Assert.Throws<ArgumentExceptionOfInvalidProperty>(MustGetFakeProperty);
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
        public TestClass(bool realProperty = false)
        {
            RealProperty = realProperty;
        }

        public bool RealProperty { get; }
    }
}