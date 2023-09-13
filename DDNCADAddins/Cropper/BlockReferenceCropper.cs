using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DDNCADAddins.Environments;
using DDNCADAddins.General;

namespace DDNCADAddins.Cropper
{
    public class BlockReferenceCropper : EntityCropper<BlockReference>
    {
        public BlockReferenceCropper(BlockReference entity, Curve boundary, WhichSideToKeep whichSideToKeep,
            CommandTransBase commandTransBase) : base(entity, boundary, whichSideToKeep, commandTransBase)
        {
        }

        internal override IEnumerable<ObjectId> Trim()
        {
            var wcsToMcs = _entity.BlockTransform.Inverse();
            var newbtrId =
                _commandTransBase.DuplicateBlockDef(_entity.BlockTableRecord,
                    GenUtils.AddTimeStampSuffix(_entity.Name));
            var newbtr = _commandTransBase.GetObjectForWrite(newbtrId) as BlockTableRecord;
            var newBoundaryId = _commandTransBase.DuplicateEntity(_boundary);
            using (var newBoundary = _commandTransBase.GetObjectForWrite(newBoundaryId) as Curve)
            {
                newBoundary.TransformBy(wcsToMcs);
                var objectIds = new List<ObjectId>();
                foreach (var id in newbtr) objectIds.Add(id);

                var result = _commandTransBase.CropEntitiesWithBoundary(objectIds, newBoundary, _whichSideToKeep);
                newBoundary.Erase(true);
                if (!result.Any())
                {
                    newbtr.Erase(true);
                    return result;
                }

                _commandTransBase.GetObjectForWrite(_entity.Id);
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