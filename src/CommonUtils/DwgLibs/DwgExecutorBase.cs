using CommonUtils.Misc;

namespace CommonUtils.DwgLibs
{
    public class DwgExecutorBase : IDwgExecutor
    {
        public IDwgCommandHelper ActiveDwgCommandHelper { get; set; }

        public DwgExecutorBase(string drawingFile = "", IMessageProvider messageProvider = null)
        {

        }
    }
}