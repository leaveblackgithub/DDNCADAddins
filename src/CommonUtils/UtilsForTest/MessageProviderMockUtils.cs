using System;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using CommonUtils.Misc;
using Moq;

namespace CommonUtils.UtilsForTest
{
    public class MessageProviderMockUtils
    {
        private static Mock<IMessageProvider> _mockInstance;

        static MessageProviderMockUtils()
        {
            _.Setup(m => m.Show(It.IsAny<string>()))
                .Callback<string>(s => LastMessage = s);
            _.Setup(m => m.Error(It.IsAny<Exception>())).Callback<Exception>(s => LastMessage = s.ToString());
            _.Setup(m => m.Error(It.IsAny<ExceptionDispatchInfo>()))
                .Callback<ExceptionDispatchInfo>(s => LastMessage = s.SourceException.ToString());
        }

        public static Mock<IMessageProvider> _ => _mockInstance ??
                                                  (_mockInstance =
                                                      new Mock<IMessageProvider>());

        private static IInvocationList MockInvocationList => _.Invocations;
        public static string LastMessage { get; private set; }
        public static IMessageProvider MessageProviderInstance => _.Object;

        public static int InvocationCount => MockInvocationList.Count;

        public static IMessageProvider NewMessageProviderInstance()
        {
            MockInvocationList.Clear();
            return MessageProviderInstance;
        }

        private static void MsgProviderInvokeClear()
        {
            MockInvocationList.Clear();
        }

        public static void MsgProviderVerifyExOnce(Expression<Action<IMessageProvider>> checkExceptAction)
        {
            _.Verify(checkExceptAction, Times.Once);
            MsgProviderInvokeClear();
        }
    }
}