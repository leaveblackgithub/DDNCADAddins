using System;

namespace CommonUtils.CustomExceptions
{
    public class TestException : Exception
    {
        public TestException(string message) : base(message)
        {
        }
    }
}