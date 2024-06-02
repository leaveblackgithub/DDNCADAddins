using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using CommonUtils.Misc;
using System;
using System.IO;
using System.Runtime.InteropServices;
using CommonUtils;
using CommonUtils.DwgLibs;
using CommonUtils.StringLibs;
using CommonUtils.CustomExceptions;

namespace ACADBase
{
    public class DwgCommandHelperBaseInAcadBase : DwgCommandHelperBase,IDwgCommandHelperInAcadBase

    {
        private Database _oldDb;

        public DwgCommandHelperBaseInAcadBase(string drawingFile = "", IMessageProvider messageProvider = null) : base(drawingFile, messageProvider)
        {
        }

        protected Document AppActiveDocument=> Application.DocumentManager.MdiActiveDocument;

        //databasehelper是和dwgcommmandhelper一一对应的指针，而这些funcs应该是databasehelper的方法。为啥搞这么复杂
        public CommandResult ExecuteDatabaseFuncs(params Func<IDatabaseHelper, CommandResult>[] databaseFuncs)
        {
            var result = new CommandResult();
            if (databaseFuncs.IsNullOrEmpty()) return result;

            acedDisableDefaultARXExceptionHandler(true);
            // Lock the document and execute the test actions.

            SaveWorkingDatabase();
            using (AppActiveDocument.LockDocument())//不管三七二十一，lock当前document
            using (var db = GetDatabaseHelper())
            {
                //exception and message has been handled in RunForEach
                result = databaseFuncs.RunForEach(db,ActiveMsgProvider);
                RestoreWorkingDatabase();
            }
            return result;
        }

        private void SaveWorkingDatabase()
        {
            _oldDb = HostApplicationServiceWrapper.GetWorkingDatabase();
        }

        private void RestoreWorkingDatabase()
        {
            if (!HostApplicationServiceWrapper.IsTargetDrawingActive(DrawingFile)) HostApplicationServiceWrapper.SetWorkingDatabase(_oldDb);
        }

        public override CommandResult ExecuteCommandInDbHelper()
        {
            var result = new CommandResult();

            acedDisableDefaultARXExceptionHandler(true);
            // Lock the document and execute the test actions.

            SaveWorkingDatabase();
            using (AppActiveDocument.LockDocument())//不管三七二十一，lock当前document
            using (var db = GetDatabaseHelper())
            {
                //exception and message has been handled in RunForEach
                result = db.ExecuteCommand();
                RestoreWorkingDatabase();
            }
            return result;

        }

        //TODO Can't verify if acedDisableDefaultARXExceptionHandler is working
            // EntryPoint may vary across autocad versions
            [DllImport("accore.dll", EntryPoint = "?acedDisableDefaultARXExceptionHandler@@YAX_N@Z")]
        public static extern void acedDisableDefaultARXExceptionHandler(bool disable);
        
        
        protected virtual IDatabaseHelper GetDatabaseHelper()
        {

#if AcConsoleTest
            return new DatabaseHelperOfAcConcole(DrawingFile);
#elif ApplicationTest
            //TODO 无法解决Application里将新开图纸激活的问题，所以Application Command需要在当前图纸解决；
            if (!HostApplicationServiceWrapper.IsTargetDrawingActive(DrawingFile)) throw ArgumentExceptionOfInvalidDwgFile._(DrawingFile);
            return new DatabaseHelperOfApplication();
#else
            return null;
#endif
        }
    }
}