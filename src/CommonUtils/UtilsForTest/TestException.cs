using System;

namespace CommonUtils.UtilsForTest
{
    public class TestException : Exception
    {
        public TestException(string message) : base(message)
        {
        }
    }
}