using System.Collections.Generic;
using System.Reflection;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using CommonUtils.Misc;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace ACADAddins.Archive
{
    public class O_DocHelper : DisposableClass
    {
        private O_EditorHelper _curEditorHelper;

        public O_DocHelper(Document acDoc)
        {
            AcDoc = acDoc;
        }

        public Document AcDoc { get; }
        public dynamic MLeaderStyleDictionaryId => AcCurDb.MLeaderStyleDictionaryId;
        public dynamic DimStyleTableId => AcCurDb.DimStyleTableId;
        public dynamic BlockTableId => AcCurDb.BlockTableId;

        public Database AcCurDb => HostApplicationServices.WorkingDatabase;

        private ObjectId CurSpaceId => AcCurDb.CurrentSpaceId;

        public O_EditorHelper CurEditorHelper =>
            _curEditorHelper ?? (_curEditorHelper =
                new O_EditorHelper(AcDoc.Editor));

        public string Name => AcDoc.Name;

        public dynamic CLayer
        {
            get => AcCurDb.Clayer;
            set => AcCurDb.Clayer = value;
        }

        public void StopHatchAssoc()
        {
            using (var trans = StartTransaction())
            {
                if (O_CadHelper.SetSystemVariable("HPASSOC", 0))
                    CurEditorHelper.WriteMessage(
                        "\n Hatches and fills are not associated with their defining boundary objects.");

                trans.Commit();
            }
        }

        public void WriteMessage(string msg)
        {
            var oUtils = AcDoc.GetType().InvokeMember("Utility", BindingFlags.GetProperty, null, AcDoc, null);
            oUtils.GetType().InvokeMember("Prompt", BindingFlags.InvokeMethod, null, oUtils, new object[] { msg });
            Application.UpdateScreen();
            //            System.Diagnostics.Trace.WriteLine(message);
            //            _acEditor.WriteMessage(message);
        }

        public Transaction StartTransaction()
        {
            return AcDoc.TransactionManager.StartTransaction();
        }


        public void Audit()
        {
            using (var trans = StartTransaction())
            {
                AcCurDb.Audit(true, false);
            }
        }

        public void PurgeAll()
        {
            var result = true;
            while (result)
            {
                result = false;
                var dicitonaryList = new List<dynamic>
                {
                    AcCurDb.GroupDictionaryId, AcCurDb.MaterialDictionaryId, AcCurDb.MLStyleDictionaryId,
                    MLeaderStyleDictionaryId,
                    AcCurDb.PlotStyleNameDictionaryId, AcCurDb.DetailViewStyleDictionaryId,
                    AcCurDb.TableStyleDictionaryId, AcCurDb.VisualStyleDictionaryId
                };

                foreach (var dict in dicitonaryList)
                    if (PurgeDictionary(dict) && !result)
                        result = true;

                var tableList = new List<dynamic>
                {
                    BlockTableId, DimStyleTableId, AcCurDb.TextStyleTableId, AcCurDb.LayerTableId,
                    AcCurDb.LinetypeTableId
                };

                foreach (var table in tableList)
                    if (PurgeTable(table) && !result)
                        result = true;
                CurEditorHelper.Regen();
            }
        }

        private bool PurgeTable(dynamic table)
        {
            dynamic acObjIdColl = new ObjectIdCollection();

            // Step through each layer and add iterator to the ObjectIdCollection
            foreach (var item in table) acObjIdColl.Add((ObjectId)item);

            // Remove the layers that are in use and return the ones that can be erased
            return PurgeObjIdColl(acObjIdColl);
        }

        private bool PurgeDictionary(dynamic table)
        {
            dynamic acObjIdColl = new ObjectIdCollection();

            // Step through each layer and add iterator to the ObjectIdCollection
            foreach (var dict in table) acObjIdColl.Add((ObjectId)dict.Value);

            // Remove the layers that are in use and return the ones that can be erased
            return PurgeObjIdColl(acObjIdColl);
        }

        private bool PurgeObjIdColl(dynamic acObjIdColl)
        {
            var result = false;
            AcCurDb.Purge(acObjIdColl);
            foreach (var acObjId in acObjIdColl)
                try
                {
                    using (var trans = StartTransaction())
                    {
                        var obj = trans.GetObject(acObjId, OpenMode.ForWrite);
                        if (obj != null)
                        {
                            var objName = "";
                            var objType = obj.GetType();
                            if (objType != null && objType.GetProperty("Name") != null) objName = $":{obj.Name}";
                            // Erase the unreferenced layer
                            obj.Erase(true);
                            CurEditorHelper.WriteMessage($"\n{O_EntExt.GetEntDxfName(obj)}{objName} deleted.");
                            if (!result) result = true;
                        }

                        trans.Commit();
                    }
                }
                catch (Exception)
                {
                }


            return result;
        }

        public void SetByLayer()
        {
            using (var trans = StartTransaction())
            {
                CurEditorHelper.SetByLayer();
            }
        }

        public void DeepCloneObjects(ObjectIdCollection identifiers, ObjectId id, IdMapping mapping,
            bool deferTranslation)
        {
            AcCurDb.DeepCloneObjects(identifiers, id, mapping, deferTranslation);
        }

        protected override void DisposeUnManaged()
        {
        }

        protected override void DisposeManaged()
        {
        }
    }
}