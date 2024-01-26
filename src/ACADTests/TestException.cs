using System;

namespace ACADTests
{
    public class TestException : Exception
    {
        public TestException(string message) : base(message)
        {
        }
    }
}