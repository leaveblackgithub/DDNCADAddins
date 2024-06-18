using ACADBase;
using CommonUtils.Misc;

namespace ACADBaseOfApplication
{
    public class AppBaseDwgCommandHelper: BaseDwgCommandHelper
    {
        public EditorWrapper ActiveEditorWrapper;
        public AppBaseDwgCommandHelper(string drawingFile = "") : base(drawingFile)
        {
        }

        public AppBaseDwgCommandHelper()
        {
        }
        protected override OperationResult<VoidValue> InitiateEnvironment()
        {
            var resultOfGettingEditor = EditorWrapper.GetActiveEditorWrapper();
            if(!resultOfGettingEditor.IsSuccess)return OperationResult<VoidValue>.Failure(ExceptionMessage.NoActiveEditor());
            ActiveEditorWrapper = resultOfGettingEditor.ReturnValue;
            return base.InitiateEnvironment();
        }
    }
}