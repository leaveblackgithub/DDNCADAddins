using Autodesk.AutoCAD.Internal;

namespace PreviousDevelopmentToRefactor.Environments
{
    public static class CadUtils
    {
        public static string GetCommandPromptString()
        {
            return Utils.GetCommandPromptString();
        }

        public static string GetLastCommandLines(int lastLines, bool ignoreNull = true)
        {
            var acCommandLineList = Utils.GetLastCommandLines(lastLines, ignoreNull);
            return string.Join("\n", acCommandLineList);
        }
    }
}