using System;
using ACADBase;
using Autodesk.AutoCAD.ApplicationServices.Core;
using CADAddins.Archive;
using CommonUtils.DwgLibs;

namespace CADAddins.Environments
{
    public class CommandBase : O_CommandBase
    {
        public IDwgCommandHelper ActiveDwgCommandHelper;

        public CommandBase()
        {
            ActiveDwgCommandHelper = new DwgCommandHelperBase("",
                new MessageProviderOfEditor(Application.DocumentManager.CurrentDocument.Editor));
        }

        public override void RunCommand()
        {
            throw new NotImplementedException();
        }
    }
}