using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using CADAddins.Archive;
using CommonUtils;

namespace CADAddins.LibsOfCleanup
{
    public class LayerHelper
    {
        private const string _layerSep = "___";
        public const string Layer0Name = "0";
        public const string LayerDefpointsName = "DEFPOINTS";

        private readonly Dictionary<string, dynamic> _cleantypes;
        private readonly O_DocHelper _curDocHelper;
        private readonly O_EditorHelper _curEditorHelper;
        private readonly LTypeHelper _curLtypeHelper;
        private readonly Dictionary<string, dynamic> _dictFrozenNpltOf0Defpoints;
        private readonly Dictionary<string, dynamic> _dictOffOf0Defpoints;
        private readonly Dictionary<string, List<string>> _dirtytypes;
        private readonly dynamic _layerTableId;
        private readonly List<string> _listFrozenNplt;
        private readonly List<dynamic> _listLayerLock;
        private readonly List<dynamic> _listLayerOffToRemoveNonBlk;
        private object dynamic;

        public LayerHelper(dynamic layerTableId, O_DocHelper docHelper, LTypeHelper ltypeHelper)
        {
            _layerTableId = layerTableId;
            _curDocHelper = docHelper;
            _curEditorHelper = _curDocHelper.CurEditorHelper;
            _curLtypeHelper = ltypeHelper;
            _listLayerOffToRemoveNonBlk = new List<dynamic>();
            if (!_curLtypeHelper.CleanTag) throw new SystemException("\nLinetypes not cleaned up.");
            _cleantypes = new Dictionary<string, dynamic>();
            _dirtytypes = new Dictionary<string, List<string>>();
            _dictFrozenNpltOf0Defpoints = new Dictionary<string, dynamic>();
            _dictOffOf0Defpoints = new Dictionary<string, dynamic>();
            _listFrozenNplt = new List<string>();
            _listLayerLock = new List<dynamic>();
        }

        public CmdCancelled Cleanup()
        {
            PrepareLayers();
            DelNonBlkEntsOnOffLayers();
            TryCleanDirtyNames();
            DelFrozenNpltLayers();
            if (MergeDirtyLayers() == CmdCancelled.Cancelled) return CmdCancelled.Cancelled;
//            _curEditorHelper.WriteMessage(_dirtytypes);
            DelEntsOnLayer0Defpoints();
            Make0CurLayer();
            return CmdCancelled.Succeeded;
        }

        public void PrepareLayers()
        {
            var layers = _layerTableId;
            using (var trans = _curDocHelper.StartTransaction())
            {
                foreach (var layer in layers)
                    try
                    {
                        string layerName = layer.Name.ToUpper();
                        layer.Name = layerName;
                        if (TryUnLock(layer)) _listLayerLock.Add(layer);
                        if (Is0OrDefpoints(layerName))
                        {
                            if (TryThaw(layer) || !layer.IsPlottable)
                            {
                                _dictFrozenNpltOf0Defpoints[layerName] = layer;
                                continue;
                            }

                            if (TryLayerOn(layer))
                            {
                                _dictOffOf0Defpoints[layerName] = layer;
                                continue;
                            }
                        }

                        if (TryThaw(layer) || !layer.IsPlottable)
                        {
                            _listFrozenNplt.Add(layerName);
                            continue;
                        }

                        if (TryLayerOn(layer)) _listLayerOffToRemoveNonBlk.Add(layer);
                        TryAddToDirtyDict(layer);
                    }
                    catch (Exception e)
                    {
                        _curEditorHelper.WriteMessage($"\n{e.Message}");
                    }

                trans.Commit();
                NotHaveLockedLayers();
            }
        }

        private bool NotHaveLockedLayers()
        {
            var lockCount = _listLayerLock.Count;
            _curEditorHelper.WriteMessage(lockCount > 0
                ? $"\nAll {lockCount} locked layers unlocked."
                : "\n No layers locked.");
            return lockCount == 0;
        }

        private void DelNonBlkEntsOnOffLayers()
        {
            foreach (var layer in _listLayerOffToRemoveNonBlk) DelNonBlkEntsOnOffLayer(layer);
        }

        public bool Check()
        {
            return NotHaveFrozenNplt()
                   && NotHaveDirtytypes()
                   && NotHave0FrozenNpltOff()
                   && NotHaveEntsOnDefpoints()
                   && NotHaveOffLayers()
                   && NotHaveLockedLayers();
        }


        private bool NotHaveDirtytypes()
        {
            var count = _dirtytypes.Count;
            _curEditorHelper.WriteMessage($"\n{count} Layernames are found with bound prefix.");
            return count == 0;
        }

        private bool NotHaveOffLayers()
        {
            var count = _listLayerOffToRemoveNonBlk.Count;
            _curEditorHelper.WriteMessage($"\n{count} Layers are found Off.");
            return count == 0;
        }

        private bool NotHaveFrozenNplt()
        {
            var count = _listFrozenNplt.Count;
            _curEditorHelper.WriteMessage($"\n{count} Layers are found frozen.");
            return count == 0;
        }

        private bool NotHaveEntsOnDefpoints()
        {
            var isEmpty = _curEditorHelper.SelectEntsOnLayer(LayerDefpointsName) == null ? "" : "not";
            _curEditorHelper.WriteMessage($"\nLayer {LayerDefpointsName} is {isEmpty} empty.");
            return isEmpty == "";
        }

        private bool NotHave0FrozenNpltOff()
        {
            var layer0 = _layerTableId[Layer0Name];
            var isFrozenNpltOff = layer0.IsFrozen || layer0.IsOff || !layer0.IsPlottable
                ? ""
                : "not";
            _curEditorHelper.WriteMessage(
                $"\nLayer {Layer0Name} is {isFrozenNpltOff} frozen or off or Nonplt");
            return isFrozenNpltOff == "not";
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
                string entLayerName = ent.Layer;

                var entLayer = _layerTableId[entLayerName];
                if (_dictFrozenNpltOf0Defpoints.ContainsKey(entLayerName))
                {
                    ent.Erase();
                    _dictFrozenNpltOf0Defpoints.Remove(entLayerName);
                    continue;
                }

                if (_dictOffOf0Defpoints.ContainsKey(entLayerName) &&
                    O_EntExt.GetEntClassName(ent) == "AcDbBlockReference")
                {
                    ent.Erase();
                    _dictOffOf0Defpoints.Remove(entLayerName);
                    continue;
                }

                if (LTypeHelper.IsByBlock(ent.Linetype))
                {
                    SetEntByLayer(ent, Layer0Name);
                    continue;
                }

                var tgtLayer = IsDirtyName(entLayer)
                    ? TryGetCleanLayer(GetNewNameOfLayer(entLayer), entLayer.Color, GetEntLtypeShtName(entLayer, ent))
                    : entLayer;
                MoveEntToLayerOfSameLtype(tgtLayer, ent);
            }


            // Update existing block references
            O_EntExt.UpdateBlockReference(blk);
            _curEditorHelper.WriteMessage($"\nBlock definition of [{blk.Name}] redefined.");
        }


        public void CleanupEntsOfNonByLayer()
        {
            var layers = _layerTableId;
            foreach (var layer in layers)
            {
                _curEditorHelper.WriteMessage($"\nCleaning up entities on Layer [{layer.Name}]...");
                MoveEntsToLayerOfSameLtype(layer);
            }
        }

        private string GetShtNameOfNewName(dynamic layer)
        {
            var ltype = layer.LinetypeObjectId;
            string ltypeName = ltype.Name;
            return layer.Name.Replace(ltypeName + _layerSep, "");
        }


        private void MoveEntsToLayerOfSameLtype(dynamic layer)
        {
            var ents = _curEditorHelper.SelectEntsOnLayerOfLTypeNotByLayer(layer.Name); //TODO:Add color option.
            if (ents == null) return;
            _curEditorHelper.WriteMessage(
                $"\nCleaning up {ents.Length} entities on layer [{layer.Name}] of non-BYLAYER linetypes...");
            using (var trans = _curDocHelper.StartTransaction())
            {
                foreach (var ent in ents) MoveEntToLayerOfSameLtype(layer, ent);
                trans.Commit();
            }
        }

        private void MoveEntToLayerOfSameLtype(dynamic layer, dynamic ent)
        {
            string tgtLayerName = "";

            if (!IsNewName(layer.Name, ent.Linetype))
            {
                var entLtypeShtName = GetEntLtypeShtName(layer, ent);
                tgtLayerName = GetNewNameOfLayer(layer, entLtypeShtName);
                TryGetCleanLayer(tgtLayerName, layer.Color, entLtypeShtName);

            }

            SetEntByLayer(ent.Id,tgtLayerName);
        }

        private void SetEntByLayer(ObjectId entId,string layerName = "")
        {

            using (var trans = _curDocHelper.StartTransaction())
            using (var layerTable = trans.GetObject(_layerTableId, OpenMode.ForRead) as LayerTable)
            using (var ent = trans.GetObject(entId, OpenMode.ForWrite) as Entity)
            {
                if (layerName != "") ent.LayerId = layerTable[layerName];
                ent.Linetype = LTypeHelper.LtypeByLayerName;
                ent.Color = Color.FromColorIndex(ColorMethod.ByLayer, 256);
                trans.Commit();
            }
        }

        private dynamic GetEntLtypeShtName(dynamic layer, dynamic ent)
        {
            string entLtypeName = ent.Linetype;
            var layerName = layer.Name;
            var entLtypeShtName = LTypeHelper.IsByLayerOrByBlock(entLtypeName)
                ? CheckFixLType(layerName)
                : BoundPrefixUtils.RemoveBoundPrefix(entLtypeName);
            return entLtypeShtName;
        }

        private dynamic TryGetCleanLayer(string tgtLayerName, Color color, string ltypeShtName = "")
        {
            dynamic result;
            using (var trans = _curDocHelper.StartTransaction())
            using (var layerTable = trans.GetObject(_layerTableId, OpenMode.ForRead) as LayerTable)
            {
                if (layerTable.Has(tgtLayerName))
                {
                    result = layerTable[tgtLayerName];
                    trans.Commit();
                    return result;
                }
                else
                {
                    return CreateNewLayer(tgtLayerName, color, ltypeShtName);
                }
            }
        }

        private dynamic CreateNewLayer(string tgtLayerName, Color color, string ltypeShtName)
        {
            dynamic resultId;
            using (var trans = _curDocHelper.StartTransaction())
                
                using (var layerTable = trans.GetObject(_layerTableId, OpenMode.ForWrite) as LayerTable)
               using (LayerTableRecord result= new LayerTableRecord())
            {
                result.Name = tgtLayerName;
                if (ltypeShtName != "")
                    result.LinetypeObjectId = _curLtypeHelper.GetLTypeByCleanName(ltypeShtName);
                result.Color = color;
                    layerTable.Add(result);
                trans.AddNewlyCreatedDBObject(result, true);
                resultId = layerTable[tgtLayerName];
                _curEditorHelper.WriteMessage($"\nNew layer [{tgtLayerName}] added.");
                trans.Commit();
                return resultId;
            }
        }

        private void With(object dynamic, object startTransaction)
        {
            throw new NotImplementedException();
        }


        private void Make0CurLayer()
        {
            if (_curDocHelper.CLayer.Name == Layer0Name) return;
            using (var trans = _curDocHelper.StartTransaction())
            {
                _curDocHelper.CLayer = _layerTableId[Layer0Name];
                trans.Commit();
            }
        }

        private void DelEntsOnLayer0Defpoints()
        {
            if (_dictFrozenNpltOf0Defpoints.Count == 0) return;
            using (var trans = _curDocHelper.StartTransaction())
            {
                var ge = _dictFrozenNpltOf0Defpoints.GetEnumerator();
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

        private CmdCancelled MergeDirtyLayers()
        {
            if (NotHaveDirtytypes()) return CmdCancelled.Succeeded;
            using (var trans = _curDocHelper.StartTransaction())
            {
                var ge = _dirtytypes.GetEnumerator();
                while (ge.MoveNext())
                {
                    var newName = ge.Current.Key;
                    var layers = ge.Current.Value;
                    if (MergeLayers(layers, _cleantypes[newName].Name) == CmdCancelled.Cancelled)
                    {
                        trans.Commit();
                        return CmdCancelled.Cancelled;
                    }

                    ;
                }

                ge.Dispose();
                trans.Commit();
            }

            return CmdCancelled.Succeeded;
        }

        private void DelFrozenNpltLayers()
        {
            if (NotHaveFrozenNplt()) return;

            using (var trans = _curDocHelper.StartTransaction())
            {
                DelLayers(_listFrozenNplt);
                trans.Commit();
            }
        }

        private CmdCancelled MergeLayers(List<string> SrclayerNames, string tgtLayerName)
        {
            var cmdlist = new List<string> { "-laymrg" };
            foreach (var layername in SrclayerNames)
            {
                if (O_CommandBase.CancelLoop(_curEditorHelper)) return CmdCancelled.Cancelled;
                cmdlist.Add("n");
                cmdlist.Add(layername);
            }

            cmdlist.Add("");

            cmdlist.Add("n");

            cmdlist.Add(tgtLayerName);
            cmdlist.Add("Y");
            _curEditorHelper.Command(cmdlist);
            _curEditorHelper.WriteMessage($"\nAll {SrclayerNames.Count} layers merged to layer [{tgtLayerName}].");
            return CmdCancelled.Succeeded;
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
            //TODO 到这死机了？
        }

        private void TryAddToDirtyDict(dynamic layer)
        {
            string layerName = layer.Name;
            if (!IsDirtyName(layer)) return;
            var newName = GetNewNameOfLayer(layer);
            if (_dirtytypes.TryGetValue(newName, out List<string> _))
                _dirtytypes[newName] = new List<string>();
            _dirtytypes[newName].Add(layerName);
        }

        private bool TryCleanDirtyNames()
        {
            var result = true;
            foreach (var dirtytype in _dirtytypes)
            {
                dynamic layer = dirtytype.Value[0];
                var layerName = layer.Name;
                var newName = dirtytype.Key;
                if (_cleantypes.ContainsKey(newName)) continue;
                if (Rename(layer) == null)
                {
                    _curEditorHelper.WriteMessage(
                        $"\nFailed to rename {layerName} to {newName}.");
                    //TODO:should log here
                    result = false;
                    continue;
                }

                _cleantypes[newName] = layer;
                _dirtytypes.Remove(layerName);
            }

            return result;
        }

        public static bool IsDirtyName(dynamic layer)
        {
            string layerName = layer.Name;
            if (Is0OrDefpoints(layerName)) return false;
            return BoundPrefixUtils.HasBoundPrefix(layerName);
        }

        public string GetNewNameFrFlds(string layerShtName, string ltyeShtName)
        {
            return string.Join(_layerSep, new List<string> { ltyeShtName, layerShtName });
        }

        public string GetNewNameOfLayer(dynamic layer, string ltypeName = "")
        {
            if (ltypeName == "") ltypeName = CheckFixLType(layer.Name);
            string layerName = layer.Name;
            if (IsNewName(layerName, ltypeName)) return layerName;
            var shortName = GetShtNameOfDirtyLayer(layerName);

            return GetNewNameFrFlds(shortName, ltypeName);
        }


        private static string GetShtNameOfDirtyLayer(string layerName)
        {
            if (Is0OrDefpoints(layerName)) return layerName;
            var shortName = BoundPrefixUtils.RemoveBoundPrefix(layerName);
            return shortName;
        }

        private bool IsNewName(string layerName, string ltypeName)
        {
            if (layerName.StartsWith(ltypeName)) return true;
            return false;
        }

        private static bool Is0OrDefpoints(string layerName)
        {
            return IsLayer0(layerName) || IsLayerDefpoints(layerName);
        }

        private static bool IsLayerDefpoints(string layerName)
        {
            return layerName == LayerDefpointsName;
        }

        private static bool IsLayer0(string layerName)
        {
            return layerName == Layer0Name;
        }

        public string CheckFixLType(string layerName)
        {
            var layer = _layerTableId[layerName];
            var ltype = layer.LinetypeObjectId; //Can't be ByLayer or ByBlock
            string ltypeName = ltype.Name;
            if (!LTypeHelper.IsDirtyName(ltype)) return ltypeName;
            var newLtype = _curLtypeHelper.GetLTypeByCleanName(ltypeName);
            layer.LinetypeObjectId = newLtype;
            string newLtypeName = newLtype.Name;
            _curEditorHelper.WriteMessage(
                $"\nLinetype of Layer [{layerName}] changed from [{ltypeName}] to [{newLtypeName}]");
            return newLtypeName;
        }

        public string DelNonBlkEntsOnOffLayer(dynamic layer)
        {
            string layerName = layer.Name;
            if (Is0OrDefpoints(layerName))
                _dictOffOf0Defpoints[layerName] = layer;
            else
                layerName = Rename(layer, AddOffSuffix(layerName)); //TODO: OFFSUFFIX LAYER?
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
            return string.Join(_layerSep, layerName, "OFF");
        }


        private bool TryUnLock(dynamic layer)
        {
            if (layer.IsLocked == false) return false;
            layer.IsLocked = false;
//            _curEditorHelper.WriteMessage($"\nLayer [{layer.Name}] unlocked.");
            return true;
        }

        private bool TryPlttable(dynamic layer)
        {
            if (IsLayerDefpoints(layer.Name)) return false;
            if (layer.IsPlottable == false) return false;
            layer.IsLocked = false;
            //            _curEditorHelper.WriteMessage($"\nLayer [{layer.Name}] unlocked.");
            return true;
        }

        private bool TryThaw(dynamic layer)
        {
            if (layer.IsFrozen == false) return false;
            layer.IsFrozen = false;
            //            _curEditorHelper.WriteMessage($"\nLayer [{layer.Name}] unlocked.");
            return true;
        }

        private bool TryLayerOn(dynamic layer)
        {
            if (layer.IsOff == false) return false;
            layer.IsOff = false;
//            _curEditorHelper.WriteMessage($"\nLayer [{layer.Name}] turned on.");
            return true;
        }

        private string Rename(dynamic layer, string newName = "")
        {
            string result = null;
            if (newName == "") newName = GetNewNameOfLayer(layer);
            string oldName = layer.Name;
            if (string.Equals(oldName, newName, StringComparison.CurrentCultureIgnoreCase)) return result;
            try
            {
                layer.Name = newName;
                _curEditorHelper.WriteMessage($"\nLayer [{oldName}] renamed to [{newName}].");
                result = newName;
            }
            catch (Exception e)
            {
                _curEditorHelper.WriteMessage($"\nLayer [{oldName}] failed to rename to [{newName}].");
                result = null;
            }

            return result;
        }
    }
}