using System;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using CommonUtils.Misc;
using Moq;

namespace CommonUtils.UtilsForTest
{
    public class MessageProviderMockUtils
    {
        static MessageProviderMockUtils()
        {
            _.Setup(m => m.Show(It.IsAny<string>()))
                .Callback<string>(s => LastMessage = s);
            _.Setup(m => m.Error(It.IsAny<Exception>())).Callback<Exception>(s => LastMessage = s?.Message ?? "");
            _.Setup(m => m.Error(It.IsAny<ExceptionDispatchInfo>()))
                .Callback<ExceptionDispatchInfo>(s => LastMessage =
                    s?.SourceException?.Message ?? "");
        }

        private static readonly Lazy<Mock<IMessageProvider>> LazyInit = new Lazy<Mock<IMessageProvider>>();
        public static Mock<IMessageProvider> _ => LazyInit.Value;

        private static IInvocationList MockInvocationList => _.Invocations;
        public static string LastMessage { get; private set; }
        public static IMessageProvider MessageProviderInstance => _.Object;

        public static IMessageProvider NewMessageProviderInstance()
        {
            MockInvocationList.Clear();
            return MessageProviderInstance;
        }

        public static void MsgProviderInvokeClear()
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