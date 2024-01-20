using System.Linq;
using System.Text.RegularExpressions;

namespace CommonUtils.LibsOfString
{
    public static class BoundPrefixUtils
    {
        private static readonly Regex BoundPrefixRegex;

        static BoundPrefixUtils()
        {
            BoundPrefixRegex = new Regex(BoundPrefixPattern);
        }

        public const string BoundPrefixPattern = @"\$\d\$";

        public static bool HasBoundPrefix(string name)
        {
            return BoundPrefixRegex.IsMatch(name);
        }

        public static string RemoveBoundPrefix(string name)
        {
            if (!BoundPrefixUtils.HasBoundPrefix(name)) return name;
            var result = BoundPrefixRegex.Split(name);
            return result.Last();
        }
    }
}