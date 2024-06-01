using System;
using CommonUtils.DwgLibs;
using CommonUtils.Misc;

namespace ACADBase
{
    public interface IDwgCommandHelperInAcadBase : IDwgCommandHelper
    {
        CommandResult ExecuteDatabaseFuncs(params Func<IDatabaseHelper, CommandResult>[] databaseFuncs);
    }
}