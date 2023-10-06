using System.Threading;
using NUnit.Framework;

namespace ACADTests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class ExampleTestsToPass
    {
        [Test]
        public void ExampleTest_Pass()
        {
            Assert.Pass("This test should always passes.");
        }
    }
}