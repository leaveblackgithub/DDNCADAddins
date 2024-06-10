#if ApplicationTest
using Autodesk.AutoCAD.EditorInput;
using CommonUtils.Misc;

namespace ACADBase
{
    public static class MessageProvider
    {

        public static IMessageProvider _ { get; } = new MessageProviderOfEditor(DocumentManagerWrapper.GetActiveEditor());
    }
}
#endif