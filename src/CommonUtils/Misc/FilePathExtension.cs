using System.IO;
using CommonUtils.StringLibs;

namespace CommonUtils.Misc
{
    public static  class FilePathExtension
    {
        public static OperationResult<VoidValue> IsDefaultOrExistingDwg(this string drawingFile)
        {
            return drawingFile == "" || (drawingFile.SubStringRight(4) == ".dwg" && File.Exists(drawingFile))
                ? OperationResult<VoidValue>.Success()
                : OperationResult<VoidValue>.Failure(drawingFile.IsNotExistingDwg());
        }
    }
}