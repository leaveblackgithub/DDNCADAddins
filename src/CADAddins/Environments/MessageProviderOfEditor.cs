using Autodesk.AutoCAD.EditorInput;
using CommonUtils.Misc;

namespace CADAddins.Environments
{
    public class MessageProviderOfEditor : MessageProviderBase
    {
        private readonly Editor _editor;

        //use editor to construct this class
        public MessageProviderOfEditor(Editor editor)
        {
            _editor = editor;
        }

        public override void Show(string message)
        {
            _editor.WriteMessage(message);
        }
    }
}