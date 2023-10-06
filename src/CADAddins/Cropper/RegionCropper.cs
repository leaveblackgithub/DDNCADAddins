using System.Linq;
using Autodesk.AutoCAD.BoundaryRepresentation;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CADAddins.Archive;
using CADAddins.Environments;

namespace CADAddins.Cropper
{
    public class RegionCropper : EntityCropper<Region>
    {
        public RegionCropper(Region entity, Curve boundary, WhichSideToKeep whichSideToKeep,
            O_CommandTransBase oCommandTransBase) : base(entity, boundary, whichSideToKeep, oCommandTransBase)
        {
        }

        internal override Point3d GetPosition()
        {
            using (var brep = new Brep(_entity))
            {
                return brep.Vertices.ElementAt(0).Point;
            }
        }
    }
}