using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using CADAddins.Archive;
using CommonUtils;
using CommonUtils.LibsOfString;

namespace CADAddins.LibsOfCleanup
{
    public class LayerHelper
    {
        private readonly O_DocHelper _curDocHelper;
        private readonly O_EditorHelper _curEditorHelper;
        private readonly LTypeHelper _curLtypeHelper;
        private readonly Dictionary<string, dynamic> _dictFrozenNpltSpecial;
        private readonly Dictionary<string, dynamic> _dictOffSpecial;
        private readonly Dictionary<string, List<string>> _dirtytypes;
        private readonly dynamic _layerTableId;

        private Dictionary<string, dynamic> _cleantypes;
        private List<string> _listFrozenNplt;
        

        public LayerHelper(dynamic layerTableId, O_DocHelper docHelper, LTypeHelper ltypeHelper)
        {
            _layerTableId = layerTableId;
            _curDocHelper = docHelper;
            _curEditorHelper = _curDocHelper.CurEditorHelper;
            _curLtypeHelper = ltypeHelper;
            if (!_curLtypeHelper.CleanTag) throw new SystemException("\nLinetypes not cleaned up.");
            _cleantypes = new Dictionary<string, dynamic>();
            _dirtytypes = new Dictionary<string, List<string>>();
            _dictFrozenNpltSpecial = new Dictionary<string, dynamic>();
            _dictOffSpecial = new Dictionary<string, dynamic>();
            _listFrozenNplt = new List<string>();
        }

        public void Cleanup()
        {
            PrepareLayers();

            DelFrozenNpltLayers();
            MergeDirtyLayers();
//            _curEditorHelper.WriteMessage(_dirtytypes);
            DelSpecialFrozenNpltLayers();
            Make0CurLayer();
        }

        public void PrepareLayers()
        {
            var layers = _layerTableId;
            var lockCount = 0;
            using (var trans = _curDocHelper.StartTransaction())
            {
                foreach (var layer in layers)
                    try
                    {
                        string layerName = MakeLayerNameUpper(layer);
                        if (UnLock(layer)) lockCount++;
                        if (layer.IsFrozen || !layer.IsPlottable)
                        {
                            layer.IsFrozen = false;
                            if (LayerNameUtils.Is0OrDefpoints(layerName))
                            {
                                _dictFrozenNpltSpecial[layerName] = layer;
                                continue;
                            }

                            _listFrozenNplt.Add(layerName);
                            continue;
                        }

                        if (layer.IsOff) layerName = DelNonBlkEntsOnOffLayer(layer);

                        string ltypeName = CheckLType(layer);
                        AddToDirtyDict(layerName, ltypeName, layer);
                    }
                    catch (Exception e)
                    {
                        _curEditorHelper.WriteMessage($"\n{e.Message}");
                    }

                trans.Commit();
                _curEditorHelper.WriteMessage(lockCount > 0
                    ? $"\nAll {lockCount} locked layers unlocked."
                    : "\n No layers locked.");
            }
        }

        private static string MakeLayerNameUpper(dynamic layer)
        {
            string upperName = layer.Name.ToUpper();
            layer.Name = upperName;
            return upperName;
        }

        public bool Check()
        {
            if (CheckFrozenNplt()&&CheckDirtytypes()) return true;
            return false;
        }

        private bool CheckDirtytypes()
        {
            if (_dirtytypes.Count == 0)
            {
                _curEditorHelper.WriteMessage("\nAll Layernames are fine without bound prefix.");
                return true;
            }

            return false;
        }

        private bool CheckFrozenNplt()
        {
            if (_listFrozenNplt.Count == 0)
            {
                _curEditorHelper.WriteMessage("\nFrozen or non-plottable layers found.");
                return true;
            }

            return false;
        }

        public void CleanupBlocks()
        {
//            MessageBox.Show($"\n{nameof(_dictOffSpecial)}:{string.Join(",",_dictOffSpecial.Values)}");
            var blks = _curDocHelper.BlockTableId;
            using (var trans = _curDocHelper.StartTransaction())
            {
                foreach (var blk in blks) CleanupBlock(blk);
                trans.Commit();
            }
        }

        private void CleanupBlock(dynamic blk)
        {
            foreach (var ent in blk)
            {
                string layerName = ent.Layer;
                if (_dictFrozenNpltSpecial.ContainsKey(layerName))
                {
                    ent.Erase();
                    continue;
                }

                if (_dictOffSpecial.ContainsKey(layerName) && O_EntExt.GetEntClassName(ent) == "AcDbBlockReference")
                {
                    ent.Erase();
                    continue;
                }

                string ltypeName = ent.Linetype;
                if (ltypeName.ToUpper() == "BYLAYER") continue;
                if (ltypeName.ToUpper() == "BYBLOCK" && layerName != "0")
                {
                    ent.Layer = "0";
                    ent.Linetype = "BYLAYER";
                    continue;
                }

                MoveEntToByLayer(layerName, ent);
            }


            // Update existing block references
            O_EntExt.UpdateBlockReference(blk);
            _curEditorHelper.WriteMessage($"\nBlock definition of [{blk.Name}] redefined.");
        }


        public void CleanupEnts()
        {
            var layers = _layerTableId;
            _cleantypes = new Dictionary<string, dynamic>();
            var layersToClean = new List<dynamic>();
            foreach (var layer in layers)
            {
//                    MessageBox.Show(layer.Name);
                _cleantypes[layer.Name] = layer;
                layersToClean.Add(layer);
            }

            foreach (var layer in layersToClean)
            {
                _curEditorHelper.WriteMessage($"\nCleaning up entities on Layer [{layer.Name}]...");
                CleanupEntsByLayer(layer);
            }
        }

        private void CleanupEntsByLayer(dynamic layer)
        {
            string layerName = layer.Name;
            var ltype = layer.LinetypeObjectId;
            string ltypeName = ltype.Name;
            MakeEntsLtypesBylayer(layerName, ltypeName);
            var layerShtName = GetShtName(layerName, ltypeName);
            Color color = layer.Color;
            MoveEntsToByLayer(layerName, layerShtName, color);
        }

        private string GetShtName(string layerName, string ltypeName)
        {
            return layerName.Replace(ltypeName + LayerNameUtils.LayerSep, "");
        }

        private void MoveEntsToByLayer(string layerName, string layerShtName, Color color)
        {
            var ents = _curEditorHelper.SelectEntsOnLayerOfLTypeNotByLayer(layerName);
            if (ents == null) return;
            _curEditorHelper.WriteMessage($"\nCleaning up {ents.Length} entities of non-BYLAYER linetypes...");
            using (var trans = _curDocHelper.StartTransaction())
            {
                foreach (var ent in ents) MoveEntToByLayer(layerShtName, color, ent);
                trans.Commit();
            }
        }

        private void MoveEntToByLayer(string layerName, dynamic ent)
        {
            var layer = _cleantypes[layerName];
            var ltype = layer.LinetypeObjectId;
            string ltypeName = ltype.Name;
            var layerShtName = GetShtName(layerName, ltypeName);
            Color color = layer.Color;
            MoveEntToByLayer(layerShtName, color, ent);
        }

        private void MoveEntToByLayer(string layerShtName, Color color, dynamic ent)
        {
            string ltypeName = ent.Linetype;
            var ltypeShtName = BoundPrefixUtils.RemoveBoundPrefix(ltypeName);
            var tgtLayerName = LayerNameUtils.AddLtypePrefixAndUpper(layerShtName, ltypeShtName);
            var layer = GetLayer(tgtLayerName, ltypeShtName, color);

            ent.Layer = tgtLayerName;
            ent.Linetype = "BYLAYER";
        }

        private dynamic GetLayer(string tgtLayerName, string ltypeShtName, Color color)
        {
            dynamic result;
            if (_cleantypes.TryGetValue(tgtLayerName, out result)) return result;
            result = new LayerTableRecord();
            result.Name = tgtLayerName;
            result.LinetypeObjectId = _curLtypeHelper.GetLTypeByCleanName(ltypeShtName);
            result.Color = color;
            _layerTableId.Add(result);
            result = _layerTableId[tgtLayerName];
            _cleantypes[tgtLayerName] = result;
            _curEditorHelper.WriteMessage($"\nNew layer [{tgtLayerName}] added.");
            return result;
        }

        private void MakeEntsLtypesBylayer(string layerName, string ltypeName)
        {
            using (var trans = _curDocHelper.StartTransaction())
            {
                var ents = _curEditorHelper.SelectEntsOnLayerOfLType(layerName, ltypeName);
                if (ents == null) return;
                O_EntExt.ChangeLType(ents, "BYLAYER");
                _curEditorHelper.WriteMessage(
                    $"\nLinetypes of {ents.Length} entities on Layer [{layerName}] changed to BYLAYER.");
                trans.Commit();
            }
        }

        private void Make0CurLayer()
        {
            if (_curDocHelper.CLayer.Name == "0") return;
            using (var trans = _curDocHelper.StartTransaction())
            {
                _curDocHelper.CLayer = _layerTableId["0"];
                trans.Commit();
            }
        }

        private void DelSpecialFrozenNpltLayers()
        {
            if (_dictFrozenNpltSpecial.Count == 0) return;
            using (var trans = _curDocHelper.StartTransaction())
            {
                var ge = _dictFrozenNpltSpecial.GetEnumerator();
                while (ge.MoveNext())
                {
                    var layerName = ge.Current.Key;
                    var ents = _curEditorHelper.SelectEntsOnLayer(layerName);
                    if (ents == null) continue;
                    O_EntExt.EraseEnts(ents);
                    _curEditorHelper.WriteMessage($"\nAll {ents.Length} entities on Layer [{layerName}] deleted.");
                }
                
                ge.Dispose();
                trans.Commit();
            }
        }

        private void MergeDirtyLayers()
        {
            if (CheckDirtytypes()) return;
            using (var trans = _curDocHelper.StartTransaction())
            {
                var ge = _dirtytypes.GetEnumerator();
                while (ge.MoveNext())
                {
                    var newName = ge.Current.Key;
                    var layers = ge.Current.Value;
                    MergeLayers(layers, _cleantypes[newName].Name);
                }

                ge.Dispose();
                trans.Commit();
            }
        }

        private void DelFrozenNpltLayers()
        {
            if(CheckFrozenNplt()) return;

            using (var trans = _curDocHelper.StartTransaction())
            {
                DelLayers(_listFrozenNplt);
                trans.Commit();
            }
        }

        private void MergeLayers(List<string> SrclayerNames, string tgtLayerName)
        {
            var cmdlist = new List<string> { "-laymrg" };
            foreach (var layername in SrclayerNames)
            {
                cmdlist.Add("n");
                cmdlist.Add(layername);
            }

            cmdlist.Add("");

            cmdlist.Add("n");

            cmdlist.Add(tgtLayerName);
            cmdlist.Add("Y");
            _curEditorHelper.Command(cmdlist);
            _curEditorHelper.WriteMessage($"\nAll {SrclayerNames.Count} layers merged to layer [{tgtLayerName}].");
        }

        private void DelLayers(List<string> layerNames)
        {
            var cmdlist = new List<string> { "-laydel" };
            foreach (var layername in layerNames)
            {
                cmdlist.Add("n");
                cmdlist.Add(layername);
            }

            cmdlist.Add("");
            cmdlist.Add("Y");
            _curEditorHelper.Command(cmdlist);
            _curEditorHelper.WriteMessage($"\nAll {layerNames.Count} frozen or non-plottable layers deleted.");
        }

        private void AddToDirtyDict(string layerName, string ltypeName, dynamic layer)
        {
            if (LayerNameUtils.Is0OrDefpoints(layerName)) return;
            var newName = LayerNameUtils.AddLtypePrefixAndUpper(layerName, ltypeName);
            List<string> dirtyLayers;
            if (!_cleantypes.ContainsKey(newName.ToUpper()))
                if (string.Equals(layerName, newName, StringComparison.CurrentCultureIgnoreCase) ||
                    Rename(layer, newName) != null)
                {
                    _cleantypes[newName.ToUpper()] = layer;
                    return;
                }

            if (!_dirtytypes.TryGetValue(newName.ToUpper(), out dirtyLayers))
            {
                _dirtytypes[newName.ToUpper()] = new List<string>();
                dirtyLayers = _dirtytypes[newName.ToUpper()];
            }

            dirtyLayers.Add(layer.Name);
        }

        public string CheckLType(dynamic layer)
        {
            var ltype = layer.LinetypeObjectId;
            string ltypeName = ltype.Name;
            if (!BoundPrefixUtils.HasBoundPrefix(ltypeName)) return ltypeName;
            var newLtype = _curLtypeHelper.GetLTypeByCleanName(ltypeName);
            layer.LinetypeObjectId = newLtype;
            string newname = newLtype.Name;
            _curEditorHelper.WriteMessage(
                $"\nLinetype of Layer [{layer.Name}] changed from [{ltypeName}] to [{newname}]");
            return newname;
        }

        public string DelNonBlkEntsOnOffLayer(dynamic layer)
        {
            LayerOn(layer);
            string layerName = layer.Name;
            if (LayerNameUtils.Is0OrDefpoints(layerName))
                _dictOffSpecial[layerName] = layer;
            else
                layerName = Rename(layer, AddOffSuffix(layerName));
            var ents = _curEditorHelper.SelectNoneBlkOnLayer(layerName);
            if (ents != null)
            {
                O_EntExt.EraseEnts(ents);
                _curEditorHelper.WriteMessage(
                    $"\nAll {ents.Length} non-block entities on Layer [{layerName}] deleted.");
            }

            return layerName;
        }

        private string AddOffSuffix(string layerName)
        {
            return string.Join(LayerNameUtils.LayerSep, layerName, "OFF");
        }


        private bool UnLock(dynamic layer)
        {
            if (layer.IsLocked == false) return false;
            layer.IsLocked = false;
//            _curEditorHelper.WriteMessage($"\nLayer [{layer.Name}] unlocked.");
            return true;
        }

        private bool LayerOn(dynamic layer)
        {
            if (layer.IsOff == false) return false;
            layer.IsOff = false;
//            _curEditorHelper.WriteMessage($"\nLayer [{layer.Name}] turned on.");
            return true;
        }

        private string Rename(dynamic layer, string newName)
        {
            string oldName = layer.Name;
            if (string.Equals(oldName, newName, StringComparison.CurrentCultureIgnoreCase)) return null;
            try
            {
                layer.Name = newName;
                _curEditorHelper.WriteMessage($"\nLayer [{oldName}] renamed to [{newName}].");
                return newName;
            }
            catch (Exception e)
            {
                _cleantypes[newName] = _layerTableId[newName];
                return null;
            }
        }
    }
}