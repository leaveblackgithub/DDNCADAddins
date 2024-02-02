namespace CommonUtils.UtilsForTest
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