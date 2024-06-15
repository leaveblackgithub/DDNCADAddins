using ACADAddins.Archive;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace ACADAddins.Cropper
{
    public class DbPointCropper : EntityCropper<DBPoint>
    {
        public DbPointCropper(DBPoint entity, Curve boundary, WhichSideToKeep whichSideToKeep,
            O_CommandTransBase oCommandTransBase) : base(entity, boundary, whichSideToKeep, oCommandTransBase)
        {
        }

        internal override Point3d GetPosition()
        {
            return _entity.Position;
        }
    }
}