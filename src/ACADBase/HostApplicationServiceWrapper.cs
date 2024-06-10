using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.Misc;

namespace ACADBase
{
    public static class HostApplicationServiceWrapper
    {
        public static Database GetWorkingDatabase()
        {
            return HostApplicationServices.WorkingDatabase;
        }

        public static void SetWorkingDatabase(Database database)
        {
            if (database != null) HostApplicationServices.WorkingDatabase = database;
        }

        // public static bool IsTargetDrawingActive(string drawingFile)
        // {
        //     return GetWorkingDatabase() != null &&
        //            (string.IsNullOrEmpty(drawingFile) || GetWorkingDatabase().Filename == drawingFile);
        // }
        public static OperationResult<VoidValue> IsTargetDrawingActive(string drawingFile)
        {
            return GetWorkingDatabase() != null &&
                   (string.IsNullOrEmpty(drawingFile) || GetWorkingDatabase().Filename == drawingFile)
                ? OperationResult<VoidValue>.Success()
                : OperationResult<VoidValue>.Failure(ExceptionMessage.NullActiveDocument(drawingFile));
        }
    }
}