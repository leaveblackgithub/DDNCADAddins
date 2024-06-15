using System.Collections.Generic;
using System.Linq;
using ACADAddins.Archive;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CommonUtils.Misc;

namespace ACADAddins.Cropper
{
    public class BlockReferenceCropper : EntityCropper<BlockReference>
    {
        public BlockReferenceCropper(BlockReference entity, Curve boundary, WhichSideToKeep whichSideToKeep,
            O_CommandTransBase oCommandTransBase) : base(entity, boundary, whichSideToKeep, oCommandTransBase)
        {
        }

        internal override IEnumerable<ObjectId> Trim()
        {
            var wcsToMcs = _entity.BlockTransform.Inverse();
            var newbtrId =
                OCommandTransBase.DuplicateBlockDef(_entity.BlockTableRecord,
                    DateTimeUtils.AddTimeStampSuffix(_entity.Name));
            var newbtr = OCommandTransBase.GetObjectForWrite(newbtrId) as BlockTableRecord;
            var newBoundaryId = OCommandTransBase.DuplicateEntity(_boundary);
            using (var newBoundary = OCommandTransBase.GetObjectForWrite(newBoundaryId) as Curve)
            {
                newBoundary.TransformBy(wcsToMcs);
                var objectIds = new List<ObjectId>();
                foreach (var id in newbtr) objectIds.Add(id);

                var result = OCommandTransBase.CropEntitiesWithBoundary(objectIds, newBoundary, _whichSideToKeep);
                newBoundary.Erase(true);
                if (!result.Any())
                {
                    newbtr.Erase(true);
                    return result;
                }

                OCommandTransBase.GetObjectForWrite(_entity.Id);
                _entity.BlockTableRecord = newbtrId;
                return new List<ObjectId> { _entity.Id };
            }
        }

        internal override Point3d GetPosition()
        {
            return _entity.Position;
        }
    }
}