using System.Collections.Generic;
using Utils = Autodesk.AutoCAD.Internal.Utils;

namespace CADAddins.Environments
{
    public static class CadUtils
    {
        public static string GetCommandPromptString()
        {
            return Utils.GetCommandPromptString();
        }

        public static string GetLastCommandLines(int lastLines,bool ignoreNull=true)
        {
            List<string> acCommandLineList = Utils.GetLastCommandLines(lastLines,ignoreNull);
            return string.Join("\n", acCommandLineList);
        }
    }
}
