using System;
using System.Reflection;
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
            Assert.True(_testClass.MustGetProperty<bool>(_realproperty,out _).IsSuccess);
            _testClass.GetObjectPropertyValue<bool>(_realproperty,out var value);
            Assert.False(value);
            FuncResult result;
            result = _testClass.MustGetProperty<string>(_realproperty,out _);
            Assert.AreEqual(result.CancelMessage, ExceptionMessage.InvalidProperty<string>(_testClass, _realproperty));
            result = _testClass.MustGetProperty<bool>(_fakeproperty, out _);
            Assert.AreEqual(result.CancelMessage, ExceptionMessage.InvalidProperty<bool>(_testClass, _fakeproperty));
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