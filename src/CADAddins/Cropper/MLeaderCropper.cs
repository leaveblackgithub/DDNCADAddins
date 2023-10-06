using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CADAddins.Archive;
using CADAddins.Environments;

namespace CADAddins.Cropper
{
    public class MLeaderCropper : EntityCropper<MLeader>
    {
        public MLeaderCropper(MLeader entity, Curve boundary, WhichSideToKeep whichSideToKeep,
            O_CommandTransBase oCommandTransBase) : base(entity, boundary, whichSideToKeep, oCommandTransBase)
        {
        }

        internal override Point3d GetPosition()
        {
            return _entity.TextLocation;
        }
    }
}