using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using CADAddins.Archive;
using CADAddins.Environments;
using CADAddins.General;

namespace CADAddins.LibsOfCleanup
{
    public class LTypeHelper:DisposableClass
    {
        public bool CleanTag { get; private set; }
        private readonly dynamic _lTypeTableId;
        private readonly O_EditorHelper _curEditorHelper;
        private O_DocHelper _curDocHelper;
        private Dictionary<string, dynamic> Cleantypes { get; set; }

        public LTypeHelper(dynamic lTypeTableId, O_DocHelper docHelper)
        {
            _lTypeTableId = lTypeTableId;
            _curDocHelper = docHelper;
            _curEditorHelper = _curDocHelper.CurEditorHelper;
            CleanTag = false;
        }

        public void Cleanup()
        {
            Cleantypes = new Dictionary<string, dynamic>();
            Dictionary<string,List<dynamic>> dirtytypes = new Dictionary<string, List<dynamic>>() ;
            var ltypes = _lTypeTableId;
            foreach (var ltype in ltypes)
            {
                string ltypeName = ltype.Name;
                if (!GenUtils.HasBoundPrefix(ltypeName))
                {
                    Cleantypes[ltypeName] = ltype;
                    continue;
                }
                string cleanname = GenUtils.RemoveBoundPrefix(ltypeName);
                if (!dirtytypes.ContainsKey(cleanname))
                {
                    dirtytypes[cleanname] = new List<dynamic>();
                }
                dirtytypes[cleanname].Add(ltype);
            }

            if (dirtytypes.Count == 0)
            {
                _curEditorHelper.WriteMessage("All linetypes are fine without bound prefix.");
                CleanTag = true;
                return;
            }

            int count = 0;
            using (Transaction trans=_curDocHelper.StartTransaction())
            {
                var ge = dirtytypes.GetEnumerator();
                while (ge.MoveNext())
                {
                    string cleanname = ge.Current.Key;
                    if (Cleantypes.ContainsKey(cleanname)) continue;
                    var ltype = ge.Current.Value[0];
                    string oldname = ltype.Name;
                    ltype.Name = cleanname;
                    _curEditorHelper.WriteMessage($"\nLinetype [{oldname}] renamed to [{cleanname}].");
                    Cleantypes[cleanname] = ltype;
                    count++;
                }
                ge.Dispose();
                CleanTag = true;
                trans.Commit();

            }
            _curEditorHelper.WriteMessage($"\n{count} Linetypes renamed in total.");
        }

        public dynamic GetLTypeByCleanName(string name)
        {
            string cleanname;
            if (!GenUtils.HasBoundPrefix(name))
            {
                cleanname = name;}
            else
            {
                cleanname = GenUtils.RemoveBoundPrefix(name);
            }

            dynamic result;
            if(!Cleantypes.TryGetValue(cleanname,out result))throw new SystemException($"Can't find Linetype[{cleanname}]");
            return result;
        }

        protected override void DisposeUnManaged()
        {
            Cleantypes.Clear();
            Cleantypes = null;
        }

        protected override void DisposeManaged()
        {
            throw new System.NotImplementedException();
        }
    }
}
