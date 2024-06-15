using System.Threading;
using NUnit.Framework;

namespace ACADTestsOfUnit.UnitTests.AcConsoleTests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class ExampleTestsToFail
    {
        [Test]
        public void ExampleTest_Fail()
        {
            Assert.Fail("This test should always fail.");
        }
    }
}