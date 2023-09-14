namespace PreviousDevelopmentToRefactor.Archive
{
    public static class O_EntExt
    {
        public static void EraseEnts(dynamic ents)
        {
            if (ents == null || ents.Length == 0) return;
            foreach (var ent in ents) ent.Erase();
        }

        public static void ChangeLType(dynamic ents, string ltypeName)
        {
            if (ents == null || ents.Length == 0) return;
            foreach (var ent in ents) ent.Linetype = ltypeName;
        }

        public static dynamic GetEntClassName(dynamic ent)
        {
            return ent.GetRXClass().Name;
        }

        public static dynamic GetEntDxfName(dynamic ent)
        {
            return ent.GetRXClass().DxfName;
        }


        public static void UpdateBlockReference(dynamic blk)
        {
            foreach (var ent in blk.GetBlockReferenceIds(false, true)) ent.RecordGraphicsModified(true);
        }
    }
}