using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CommonUtils.LibsOfString
{
    public class LayerNameUtils
    {
        public const string Layer0 = "0";
        public const string LayerDefpoints = "DEFPOINTS";
        public const string LayerSep = "___";

        public static string AddLtypePrefixAndUpper(string layerName, string ltypeName)
        {
            layerName = layerName.ToUpper();
            ltypeName = ltypeName.ToUpper();
            if (IsCorrectPattern(layerName, ltypeName)) return layerName;
            return string.Join(LayerSep, ltypeName, GetUpperShortName(layerName));
        }

        public static bool IsCorrectPattern(string layerName, string ltypeName)
        {
            return !BoundPrefixUtils.HasBoundPrefix(layerName)
                   &&StringUtils.StartsWithIgnoreCase(layerName,ltypeName,LayerSep)
                   &&layerName==layerName.ToUpper();
        }

        public static string GetUpperShortName(string layerName)
        {
            string upperShortName = layerName.ToUpper();
            if(BoundPrefixUtils.HasBoundPrefix(upperShortName))
                upperShortName=BoundPrefixUtils.RemoveBoundPrefix(upperShortName);
            if (upperShortName.Contains(LayerSep)) upperShortName=upperShortName.SplitByString(LayerSep)[1];
            return upperShortName;
        }
        public static bool Is0OrDefpoints(string layerName)
        {
            return IsLayer0(layerName) || IsLayerDefPoints(layerName);
        }

        public static bool IsLayerDefPoints(string layerName)
        {
            return StringUtils.EqualsIgnoreCase(layerName,LayerDefpoints);

        }

        public static bool IsLayer0(string layerName)
        {
            return StringUtils.EqualsIgnoreCase(layerName, Layer0);
        }
    }
} 