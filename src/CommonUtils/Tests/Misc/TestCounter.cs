using CommonUtils.CustomExceptions;
using CommonUtils.Misc;

namespace CommonUtils.Tests.Misc
{
    public class TestCounter
    {
        public TestCounter()
        {
            Count = 0;
        }

        public int Count { get; set; }

        public void Increment()
        {
            Count++;
        }

        public void Clear()
        {
            Count = 0;
        }
    }
}