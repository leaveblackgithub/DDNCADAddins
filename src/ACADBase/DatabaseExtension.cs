using System;
using Autodesk.AutoCAD.DatabaseServices;

namespace ACADBase
{
    public static class DatabaseExtension
    {
        public static Database NewDrawingDatabase()
        {
            return new Database(true, true);
        }

        public static bool IsDataBaseSavedAsDwg(this Database database)
        {
            return database != null && database.Filename.EndsWith("dwg");
        }

        //TODO处理文件已开启情况
        public static Database GetDwgDatabase(string drawingFile)
        {
            var database = default(Database);
            try
            {
                database = NewDrawingDatabase();
                database.ReadDwgFile(drawingFile, FileOpenMode.OpenTryForReadShare, true, null);
            }
            catch (Exception e)
            {
                database = null;
            }

            return database;
        }
    }
}