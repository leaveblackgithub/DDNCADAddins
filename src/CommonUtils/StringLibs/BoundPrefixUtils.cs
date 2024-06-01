using System.Linq;
using System.Text.RegularExpressions;

namespace CommonUtils.StringLibs
{
    public static class BoundPrefixUtils
    {
        public const string BoundPrefixPattern = @"\$\d\$";
        private static readonly Regex BoundPrefixRegex;

        static BoundPrefixUtils()
        {
            BoundPrefixRegex = new Regex(BoundPrefixPattern);
        }

        public static bool HasBoundPrefix(string name)
        {
            return BoundPrefixRegex.IsMatch(name);
        }

        public static string RemoveBoundPrefix(string name)
        {
            if (!HasBoundPrefix(name)) return name;
            var result = BoundPrefixRegex.Split(name);
            return result.Last();
        }
    }
}