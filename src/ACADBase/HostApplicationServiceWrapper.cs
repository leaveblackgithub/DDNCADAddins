using Autodesk.AutoCAD.DatabaseServices;

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
            if(database!=null) HostApplicationServices.WorkingDatabase= database;
        }
        public static bool IsTargetDrawingActive(string drawingFile)
        {
            return GetWorkingDatabase() != null &&
                   (string.IsNullOrEmpty(drawingFile) || GetWorkingDatabase().Filename == drawingFile);
        }
    }
}