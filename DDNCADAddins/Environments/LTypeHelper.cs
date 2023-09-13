using System.Collections.Generic;
using CADCleanup.Environments;
using General;

namespace CADCleanup.LibsOfCleanup
{
    public class LTypeHelper:DisposableClass
    {
        private dynamic _lTypeTableId;
        private EditorHelper _curEditorHelper;

        public LTypeHelper(dynamic lTypeTableId, EditorHelper editorHelper)
        {
            _lTypeTableId = lTypeTableId;
            _curEditorHelper = editorHelper;
        }

        public void Cleanup()
        {
            Dictionary<string, dynamic> cleantypes = new Dictionary<string, dynamic>();
            Dictionary<string,List<dynamic>> dirtytypes = new Dictionary<string, List<dynamic>>() ;
            var ltypes = _lTypeTableId;
            foreach (var ltype in ltypes)
            {
                string ltypeName = ltype.Name;
                if (!GenUtils.HasBoundPrefix(ltypeName))
                {
                    cleantypes[ltypeName] = ltype;
                    continue;
                }
                string cleanname = GenUtils.RemoveBoundPrefix(ltypeName);
                if (!dirtytypes.ContainsKey(cleanname))
                {
                    dirtytypes[cleanname] = new List<dynamic>();
                }
                dirtytypes[cleanname].Add(ltype);
            }

            int count = 0;
            foreach (KeyValuePair<string, List<dynamic>> pair in dirtytypes)
            {
                if(cleantypes.ContainsKey(pair.Key))continue;
                string oldname = pair.Value[0].Name;
                pair.Value[0].Name = pair.Key;
                _curEditorHelper.WriteMessage($"\nLinetype [{oldname}] renamed to [{pair.Key}].");
                count++;
            }
            _curEditorHelper.WriteMessage($"\n{count} Linetypes renamed in total.");
        }

        public override void DisposeManaged()
        {
        }

        public override void DisposeUnManaged()
        {
        }
    }
}
