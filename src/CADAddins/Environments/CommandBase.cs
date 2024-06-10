using System;
using ACADBase;
using Autodesk.AutoCAD.ApplicationServices.Core;
using CADAddins.Archive;
using CommonUtils.DwgLibs;

namespace CADAddins.Environments
{
    public class CommandBase : O_CommandBase
    {
        public DwgCommandHelperBase ActiveDwgCommandHelper;

        public CommandBase()
        {
            ActiveDwgCommandHelper = new DwgCommandHelperBase("");
        }

        public override void RunCommand()
        {
            throw new NotImplementedException();
        }
    }
}