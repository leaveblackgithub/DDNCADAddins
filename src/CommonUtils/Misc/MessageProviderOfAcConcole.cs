using System.Windows.Forms;

namespace CommonUtils.Misc
{
    public class MessageProviderOfAcConcole : MessageProviderBase
    {
        public override void Show(string message)
        {
            MessageRecent._ = message;
        }
    }
}