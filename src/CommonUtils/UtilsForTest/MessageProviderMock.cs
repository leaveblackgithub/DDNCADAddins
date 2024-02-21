using System;
using CommonUtils.Misc;
using Moq;

namespace CommonUtils.UtilsForTest
{
    //TODO: Single instance
    public class MessageProviderMock : MessageProviderBase
    {
        private MessageProviderMock()
        {
            ThisMock = new Mock<IMessageProvider>();
        }

        public static MessageProviderMock _ { get; } = new MessageProviderMock();

        public Mock<IMessageProvider> ThisMock { get; set; }

        public override void Show(string message)
        {
            ThisMock.Object.Show(message);
        }
    }
}