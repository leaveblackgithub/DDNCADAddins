using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CADAddins.Environments;
using CADAddins.General;

namespace CADAddins.Cropper
{
    public class BlockReferenceCropper : EntityCropper<BlockReference>
    {
        public BlockReferenceCropper(BlockReference entity, Curve boundary, WhichSideToKeep whichSideToKeep,
            CommandTransBase commandTransBase) : base(entity, boundary, whichSideToKeep, commandTransBase)
        {
        }

        internal override IEnumerable<ObjectId> Trim()
        {
            Matrix3d wcsToMcs = _entity.BlockTransform.Inverse();
            ObjectId newbtrId =
                _commandTransBase.DuplicateBlockDef(_entity.BlockTableRecord,
                    GenUtils.AddTimeStampSuffix(_entity.Name));
            BlockTableRecord newbtr = _commandTransBase.GetObjectForWrite(newbtrId) as BlockTableRecord;
            ObjectId newBoundaryId = _commandTransBase.DuplicateEntity(_boundary);
            using (Curve newBoundary = _commandTransBase.GetObjectForWrite(newBoundaryId) as Curve)
            {
                newBoundary.TransformBy(wcsToMcs);
                List<ObjectId> objectIds = new List<ObjectId>();
                foreach (ObjectId id in newbtr)
                {
                    objectIds.Add(id);
                }

                IEnumerable<ObjectId> result = _commandTransBase.CropEntitiesWithBoundary(objectIds, newBoundary, _whichSideToKeep);
                newBoundary.Erase(true);
                if (!result.Any())
                {
                    newbtr.Erase(true);
                    return result;
                }

                _commandTransBase.GetObjectForWrite(_entity.Id);
                _entity.BlockTableRecord = newbtrId;
                return new List<ObjectId>(){_entity.Id};
            }
        }

        internal override Point3d GetPosition()
        {
            return _entity.Position;
        }
    }
}