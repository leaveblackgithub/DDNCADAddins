using System;
using System.IO;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.Misc;

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
            return database!=null&&database.Filename.EndsWith("dwg");
        }

        public static Database GetDwgDatabase(string drawingFile)
        {
            Database database = NewDrawingDatabase();
            try
            {
                database.ReadDwgFile(drawingFile, FileOpenMode.OpenForReadAndWriteNoShare, true, null);
            }
            catch (Exception e)
            {
                database = null;
            }
            return database;
        }
    }
}