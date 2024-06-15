namespace CommonUtils.Misc
{
    public interface IMessageProvider
    {
        string RecentMessage { get; }
        void WriteMessage(string message);
    }
}