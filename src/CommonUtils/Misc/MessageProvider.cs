#if AcConsoleTest
namespace CommonUtils.Misc
{
    public static class MessageProvider
    {

        public static IMessageProvider _ { get; } = new MessageProviderOfAcConcole();
    }
}
#endif