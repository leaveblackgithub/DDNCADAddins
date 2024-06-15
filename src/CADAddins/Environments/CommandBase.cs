using System;
using ACADBase;
using CADAddins.Archive;

namespace CADAddins.Environments
{
    public class CommandBase : O_CommandBase
    {
        public BaseDwgCommandHelper ActiveDwgCommandHelper;

        public CommandBase()
        {
            ActiveDwgCommandHelper = new BaseDwgCommandHelper("");
        }

        public override void RunCommand()
        {
            throw new NotImplementedException();
        }
    }
}