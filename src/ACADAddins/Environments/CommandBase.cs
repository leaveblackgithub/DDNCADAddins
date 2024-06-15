using System;
using ACADAddins.Archive;
using ACADBase;

namespace ACADAddins.Environments
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