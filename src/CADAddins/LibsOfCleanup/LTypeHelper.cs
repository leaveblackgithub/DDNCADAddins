using System;
using System.Collections.Generic;
using CADAddins.Archive;
using CommonUtils;

namespace CADAddins.LibsOfCleanup
{
    public class LTypeHelper : DisposableClass
    {
        private readonly O_DocHelper _curDocHelper;
        private readonly O_EditorHelper _curEditorHelper;
        private readonly dynamic _lTypeTableId;

        public LTypeHelper(dynamic lTypeTableId, O_DocHelper docHelper)
        {
            _lTypeTableId = lTypeTableId;
            _curDocHelper = docHelper;
            _curEditorHelper = _curDocHelper.CurEditorHelper;
            CleanTag = false;
        }

        public bool CleanTag { get; private set; }
        private Dictionary<string, dynamic> Cleantypes { get; set; }

        public void Cleanup()
        {
            Cleantypes = new Dictionary<string, dynamic>();
            var dirtytypes = new Dictionary<string, List<dynamic>>();
            var ltypes = _lTypeTableId;
            foreach (var ltype in ltypes)
            {
                string ltypeName = ltype.Name;
                if (!BoundPrefixUtils.HasBoundPrefix(ltypeName))
                {
                    Cleantypes[ltypeName] = ltype;
                    continue;
                }

                var cleanname = BoundPrefixUtils.RemoveBoundPrefix(ltypeName);
                if (!dirtytypes.ContainsKey(cleanname)) dirtytypes[cleanname] = new List<dynamic>();
                dirtytypes[cleanname].Add(ltype);
            }

            if (dirtytypes.Count == 0)
            {
                _curEditorHelper.WriteMessage("All linetypes are fine without bound prefix.");
                CleanTag = true;
                return;
            }

            var count = 0;
            using (var trans = _curDocHelper.StartTransaction())
            {
                var ge = dirtytypes.GetEnumerator();
                while (ge.MoveNext())
                {
                    var cleanname = ge.Current.Key;
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
            if (!BoundPrefixUtils.HasBoundPrefix(name))
                cleanname = name;
            else
                cleanname = BoundPrefixUtils.RemoveBoundPrefix(name);

            dynamic result;
            if (!Cleantypes.TryGetValue(cleanname, out result))
                throw new SystemException($"Can't find Linetype[{cleanname}]");
            return result;
        }

        protected override void DisposeUnManaged()
        {
            Cleantypes.Clear();
            Cleantypes = null;
        }

        protected override void DisposeManaged()
        {
            throw new NotImplementedException();
        }
    }
}