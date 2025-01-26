using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CADAddins.Environments;

namespace CADAddins.Cropper
{
    public class DbPointCropper : EntityCropper<DBPoint>
    {
        public DbPointCropper(DBPoint entity, Curve boundary, WhichSideToKeep whichSideToKeep,
            CommandTransBase commandTransBase) : base(entity, boundary, whichSideToKeep, commandTransBase)
        {
        }

        internal override Point3d GetPosition()
        {
            return _entity.Position;
        }
    }
}