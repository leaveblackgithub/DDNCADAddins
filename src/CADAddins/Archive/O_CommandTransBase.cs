using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CADAddins.Cropper;
using CADAddins.Environments;
using CommonUtils.Misc;

namespace CADAddins.Archive
{
    public abstract class O_CommandTransBase : DisposableClass
    {
        public O_CommandTransBase(Transaction acTrans)
        {
            AcTrans = acTrans;
            CurOEditorHelper2.WriteMessage($"\nWorking at Drawing [{CurODocHelper2.Name}]...");
        }

        public Transaction AcTrans { get; }
        public O_DocHelper2 CurODocHelper2 => O_CadHelper2.CurODocHelper2;
        public Database AcCurDb => CurODocHelper2.AcCurDb;
        public O_EditorHelper2 CurOEditorHelper2 => CurODocHelper2.CurOEditorHelper2;
        public BlockTable CurBlockTableForRead => GetObjectForRead(CurODocHelper2.BlockTableId) as BlockTable;
        public BlockTable CurBlockTableForWrite => GetObjectForWrite(CurODocHelper2.BlockTableId) as BlockTable;
        public BlockTableRecord CurSpaceForWrite => GetObjectForWrite(CurODocHelper2.CurSpaceId) as BlockTableRecord;

        public abstract bool Run();

        public DBObject GetObjectForRead(ObjectId id)
        {
            return AcTrans.GetObjectForRead(id);
        }

        public DBObject GetObjectForWrite(ObjectId id)
        {
            return AcTrans.GetObjectForWrite(id);
        }

        public void AddNewlyCreatedDBObject(DBObject obj, bool add = true)
        {
            AcTrans.AddNewlyCreatedDBObject(obj, add);
        }

        public void AddEntToBlockTableRecord(Entity ent, ObjectId blockTableRecordId)
        {
            var btr = GetObjectForWrite(blockTableRecordId) as BlockTableRecord;
            btr.AppendEntity(ent);
            AddNewlyCreatedDBObject(ent);
        }

        public void AddEntsToBlockTableRecord(IEnumerable<Entity> ents, ObjectId blockTableRecordId)
        {
            foreach (var ent in ents) AddEntToBlockTableRecord(ent, blockTableRecordId);
        }


        public void AddPointsToBlockTableRecord(Point3dCollection pts, ObjectId blockTableRecordId)
        {
            foreach (Point3d pt in pts)
                using (var dbPoint = new DBPoint(pt))
                {
                    AddEntToBlockTableRecord(dbPoint, blockTableRecordId);
                }
        }

        protected override void DisposeUnManaged()
        {
        }

        protected override void DisposeManaged()
        {
        }

        public void DeepCloneObjects(ObjectIdCollection identifiers, ObjectId id, IdMapping mapping,
            bool deferTranslation)
        {
            AcCurDb.DeepCloneObjects(identifiers, id, mapping, deferTranslation);
        }

        public ObjectId DuplicateBlockDef(ObjectId btrId, string newname)
        {
            var btr = GetObjectForRead(btrId) as BlockTableRecord;
            if (CurBlockTableForRead.Has(newname))
            {
                CurOEditorHelper2.WriteMessage($"\nBlock with name: {newname} already exist, try again");
                return ObjectId.Null;
            }

            var ids = new ObjectIdCollection();
            foreach (var id in btr) ids.Add(id);

            using (var newbtr = new BlockTableRecord())
            {
                newbtr.Name = newname;
                var newBtrId = CurBlockTableForWrite.Add(newbtr);
                AddNewlyCreatedDBObject(newbtr);
                //----------------------------------------------------------------//


                var idMap = new IdMapping();
                DeepCloneObjects(ids, newBtrId, idMap, true);


                return newBtrId;
            }
        }

        public bool DuplicateBlockDefAndAssign(BlockReference bref, string newname)
        {
            var newbtrId = DuplicateBlockDef(bref.BlockTableRecord, newname);
            if (newbtrId == ObjectId.Null) return false;
            GetObjectForWrite(bref.Id);
            //Change BlockReference
            bref.BlockTableRecord = newbtrId;
            return true;
        }

        public ObjectId DuplicateEntity(Entity entity)
        {
            using (var ids = new ObjectIdCollection { entity.Id })
            {
                var idMap = new IdMapping();
                var curSpace = CurSpaceForWrite;
                DeepCloneObjects(ids, curSpace.Id, idMap, false);
                return idMap[entity.Id].Value;
            }
        }

        public IEnumerable<ObjectId> CropEntitiesWithBoundary(IEnumerable<ObjectId> objectIds, Curve boundary,
            WhichSideToKeep whichSideToKeep)
        {
            IEnumerable<ObjectId> result = new List<ObjectId>();
            foreach (var id in objectIds)
            {
                var dbObj = GetObjectForRead(id);
                if (!(dbObj is Entity)) continue;
                var ent = dbObj as Entity;
                //                CurEditorHelper.WriteMessage($"\n{dbObj.GetType().Name}:{dbObj.Id.ToString()}");
                var cropper = EntityCropper<Entity>.NewEntityCropper(ent, boundary, whichSideToKeep, this);
                if (cropper == null) break;
                result.Concat(cropper.Crop());
            }

            return result;
        }
    }
}