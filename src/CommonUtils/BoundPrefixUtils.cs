using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CommonUtils
{
    public static class BoundPrefixUtils
    {
        public static bool HasBoundPrefix( string name)
        {
            var regex = new Regex(@"\$\d\$");
            return regex.IsMatch(name);
        }


        public static string RemoveBoundPrefix( string name)
        {
            if (!HasBoundPrefix(name)) return name;
            var regex = new Regex(@"\$\d\$");
            var result = regex.Split(name);
            return result.Last();
        }
    }
}