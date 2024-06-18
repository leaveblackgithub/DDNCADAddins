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
        private readonly string _realmethod= "RealMethod";
        private readonly string _fakemethod = "FakeMethod";
        private readonly string _readonlyproperty = "ReadOnlyProperty";

        [Test]
        public void TryGetPropertyTest()
        {
            var result1 = _testClass.TryGetPropertyOfSpecificType<bool>(_realproperty);
            Assert.True(result1.IsSuccess);
            Assert.NotNull(result1.ReturnValue);
            var result2 = _testClass.TryGetPropertyOfSpecificType<string>(_realproperty);
            Assert.False(result2.IsSuccess);
            var result3 = _testClass.TryGetPropertyOfSpecificType<bool>(_fakeproperty);
            Assert.False(result3.IsSuccess);
        }
        [Test]
        public void GetObjectPropertyValueTest()
        {
            var result1 = _testClass.GetObjectPropertyValue<bool>(_realproperty);
            Assert.True(result1.IsSuccess);

            Assert.False(result1.ReturnValue);
            var result2 = _testClass.GetObjectPropertyValue<string>(_realproperty);
            Assert.False(result2.IsSuccess);
            var result3 = _testClass.GetObjectPropertyValue<bool>(_fakeproperty);
            Assert.False(result3.IsSuccess);
        }
        [Test]
        public void SetObjectPropertyValueTest()
        {
            var result1 = _testClass.SetObjectPropertyValue<bool>(_realproperty, true);
            Assert.True(result1.IsSuccess);
            Assert.True(_testClass.RealProperty);
            var result2 = _testClass.SetObjectPropertyValue(_fakeproperty, true);
            Assert.False(result2.IsSuccess);
            var result3 = _testClass.SetObjectPropertyValue(_readonlyproperty, true);
            Assert.False(result3.IsSuccess);
        }
        [Test]
        public void TryGeMethodOfSpecificTypeTest()
        {
            var result1 = _testClass.TryGeMethodOfSpecificType<OperationResult<VoidValue>>(_realmethod,new Type[]{typeof(string)});
            Assert.True(result1.IsSuccess);
            Assert.AreEqual(result1.ReturnValue.ReturnType,typeof(OperationResult<VoidValue>));
            var result2 = _testClass.TryGeMethodOfSpecificType<bool>(_fakemethod,Type.EmptyTypes);
            Assert.False(result2.IsSuccess);
        }
        [Test]
        public void MethodInvokeTest()
        {
            var result1 = _testClass.MethodInvoke<OperationResult<VoidValue>>(_realmethod, new object[] { "" });
            Assert.True(result1.IsSuccess);
            Assert.AreEqual(result1.ReturnValue.GetType(), typeof(OperationResult<VoidValue>));
        }
        public class TestClass
        {
            public TestClass(bool realProperty = false)
            {
                RealProperty = realProperty;
                ReadOnlyProperty = true;
            }

            public bool RealProperty { get; set; }
            public bool ReadOnlyProperty { get;  }

            public OperationResult<VoidValue> RealMethod(string param1)
            {
                return OperationResult<VoidValue>.Success();
            }
        }
    }
}