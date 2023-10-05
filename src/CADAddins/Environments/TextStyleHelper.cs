using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using CADAddins.Archive;
using General;

namespace CADAddins.Environments
{
    public class TextStyleHelper
    {
        private const string NameOfTextStyleId = "TextStyleId";
        private readonly O_DocHelper _curDocHelper;
        private readonly O_EditorHelper _curEditorHelper;
        private readonly dynamic _textStyleTableId;

        public TextStyleHelper(dynamic textStyleTableId, O_DocHelper docHelper)
        {
            _textStyleTableId = textStyleTableId;
            _curDocHelper = docHelper;
            _curEditorHelper = _curDocHelper.CurEditorHelper;
        }

        private Dictionary<string, dynamic> DirtyDict { get; set; }

        private Dictionary<string, List<dynamic>> StylesToClean { get; set; }
        private Dictionary<string, dynamic> CleanDict { get; set; }

        public void Cleanup()
        {
            GetNewNames();
            var ge = StylesToClean.GetEnumerator();
            DirtyDict = new Dictionary<string, dynamic>();
            while (ge.MoveNext())
            {
                var sameStyles = ge.Current.Value;
                var newName = ge.Current.Key;
                dynamic newStyle;
                if (!CleanDict.TryGetValue(newName, out newStyle))
                {
                    newStyle = sameStyles[0];
                    string oldName = newStyle.Name;
                    newStyle.Name = newName;
                    _curEditorHelper.WriteMessage($"\nTextstyle [{oldName}] renamed to [{newName}].");
                    CleanDict[newName] = newStyle;
                    sameStyles = sameStyles.Skip(1).ToList();
                }

                foreach (var style in sameStyles)
                {
                    DirtyDict[style.ToString()] = newStyle;
                    string styleName = style.Name;
                    ChangeEntsToTextStyle(styleName, newStyle);
                }
            }

            ge.Dispose();
            CleanupTextStypesInBlks();
//            CleanupTextStypesInDimStyles();
            CleanupTextStypesInMLeaderStyles();
        }

        private void CleanupTextStypesInMLeaderStyles()
        {
            var styles = _curDocHelper.MLeaderStyleDictionaryId;
            foreach (var item in styles)
            {
                var style = item.Value;
                var txtStyle = style.TextStyleId;
                if (txtStyle == null) continue;
                string idString = style.ToString();
                if (!DirtyDict.TryGetValue(idString, out var newTextStyle)) continue;
                style.TextStyleId = newTextStyle;
                _curEditorHelper.WriteMessage(
                    $"\nTextStyles of MLeaderStyle [{style.Name}] changed to [{newTextStyle.Name}].");
            }
        }

        private void CleanupTextStypesInDimStyles()
        {
            var dimStyles = _curDocHelper.DimStyleTableId;
            foreach (var dimStyleId in dimStyles)
                using (var trans = _curDocHelper.StartTransaction())
                {
                    DimStyleTableRecord styleTableRecord = trans.GetObject(dimStyleId, OpenMode.ForRead);
                    dynamic style = styleTableRecord.Dimtxsty;
                    string idString = style.ToString();
                    if (!DirtyDict.TryGetValue(idString, out var newTextStyle)) continue;
                    trans.GetObject(dimStyleId, OpenMode.ForWrite);
                    styleTableRecord.Dimtxsty = newTextStyle;
                    _curEditorHelper.WriteMessage(
                        $"\nTextStyles of DimStyle[{styleTableRecord.Name}] changed to [{newTextStyle.Name}].");
                    trans.Commit();
                }
        }

        private void CleanupTextStypesInBlks()
        {
            var blks = _curDocHelper.BlockTableId;
            foreach (var blkId in blks)
                using (var trans = _curDocHelper.StartTransaction())
                {
                    BlockTableRecord blk = trans.GetObject(blkId, OpenMode.ForRead);
                    if (blk.IsFromExternalReference || blk.IsLayout) continue;
                    //            _curEditorHelper.WriteMessage($"\n{blk.Name}");
                    trans.GetObject(blkId, OpenMode.ForWrite);
                    var count = 0;
                    if (blk.Name == "*D1687") _curEditorHelper.WriteMessage($"\n{blk.Name}");
                    foreach (dynamic objId in blk)
                    {
                        DBObject obj = trans.GetObject(objId, OpenMode.ForWrite);
                        if (obj.ContainProperty(NameOfTextStyleId))
                            //                            ||
                            //                            entClassName == "AcDbAttributeDefinition")
                        {
//                            trans.GetObject(objId, OpenMode.ForWrite);
                            //                            _curEditorHelper.WriteMessage($"\n{entClassName}:{ent.TextStyleId.Name}");

                            dynamic textStyleId = GenUtils.GetObjectPropertyValue(obj, NameOfTextStyleId);
                            if (textStyleId == null) continue;
                            string idString = textStyleId.ToString();
                            if (!DirtyDict.TryGetValue(idString, out var newTextStyle)) continue;
                            try
                            {
                                GenUtils.SetObjectPropertyValue(obj, NameOfTextStyleId, newTextStyle);
                                count++;
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show($"\n{blk.Name}:{obj.GetType().Name}");
                            }
                        }
                    }

                    if (count > 0)
                    {
                        O_EntExt.UpdateBlockReference(blk);
                        _curEditorHelper.WriteMessage(
                            $"\nTextStyles of {count} Entities in Block [{blk.Name}] changed to clean type.");
                    }

                    trans.Commit();
                }
        }

        private void ChangeEntsToTextStyle(string oldName, dynamic newStyle)
        {
            var ents = _curEditorHelper.SelectOfTextStyle(oldName);
            if (ents == null) return;
            foreach (var ent in ents) ent.TextStyleId = newStyle;
            _curEditorHelper.WriteMessage(
                $"\n{ents.Length} entities changed from textstyle [{oldName}] to [{newStyle.Name}].");
        }

        private void GetNewNames()
        {
            var textstyles = _textStyleTableId;
            StylesToClean = new Dictionary<string, List<dynamic>>();
            CleanDict = new Dictionary<string, dynamic>();
            foreach (var textstyle in textstyles)
            {
                var styleDictionary = new Dictionary<string, object>();
                styleDictionary["Font"] = textstyle.Font.TypeFace; //Font.TypeFace;
                styleDictionary["FileName"] = textstyle.FileName; //Font.TypeFace;
                styleDictionary["BigFontFileName"] = textstyle.BigFontFileName ?? "";
                styleDictionary["XScale"] = Math.Round(textstyle.XScale, 2);
                styleDictionary["TextSize"] = Math.Round(textstyle.TextSize, 2);
                styleDictionary["Annotative"] = (int)textstyle.Annotative;
                styleDictionary["FlagBits"] = textstyle.FlagBits;
                styleDictionary["IsVertical"] = Convert.ToInt32(textstyle.IsVertical);
                styleDictionary["ObliquingAngle"] = Math.Round(textstyle.ObliquingAngle, 2);
//                _curEditorHelper.WriteMessage($"\n{textstyle.Name}:");
                var newname = string.Join("$", styleDictionary.Values).Replace(".", "_").ToUpper();
                List<dynamic> sameStyles;
                if (string.Equals(textstyle.Name, newname, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (!CleanDict.ContainsKey(newname)) CleanDict[newname] = textstyle;
                    continue;
                }

                if (!StylesToClean.TryGetValue(newname, out sameStyles))
                {
                    StylesToClean[newname] = new List<dynamic>();
                    sameStyles = StylesToClean[newname];
                }

                sameStyles.Add(textstyle);
            }
        }
    }
}