using System;
using CommonUtils.Misc;
using Moq;

namespace CommonUtils.UtilsForTest
{
    public class MessageProviderMock : MessageProviderBase
    {
        public MessageProviderMock()
        {
            ThisMock = new Mock<IMessageProvider>();
        }

        public Mock<IMessageProvider> ThisMock { get; set; }

        public override void Show(string message)
        {
            ThisMock.Object.Show(message);
        }
    }
}