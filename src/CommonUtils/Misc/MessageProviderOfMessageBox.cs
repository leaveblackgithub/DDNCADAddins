using System;
using System.Windows.Forms;
using NLog;

namespace CommonUtils.Misc
{
    public class MessageProviderOfMessageBox : MessageProviderBase
    {
        public override void Show(string message)
        {
            MessageBox.Show(message);
        }
    }
}