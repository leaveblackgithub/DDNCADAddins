using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;

namespace CADAddins
{
    public static class HostApplicationServicesExtensions
    {
        public static bool UserBreakWithMessagePump(this HostApplicationServices hostapp)
        {
            Application.DoEvents();
            return hostapp.UserBreak();
        }
    }
}