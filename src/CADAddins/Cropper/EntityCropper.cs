using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CADAddins.Archive;

namespace CADAddins.Cropper
{
    public enum WhichSideToKeep
    {
        Outside,
        Inside
    }

    public interface EntityCropperInterface<out T> where T : Entity
    {
        IEnumerable<ObjectId> Crop();
    }

    public class EntityCropper<T> : EntityCropperInterface<T> where T : Entity
    {
        internal Curve _boundary;
        internal T _entity;
        internal Point3dCollection _ptIntersects;
        internal WhichSideToKeep _whichSideToKeep;
        internal string _whichSideToKeepString;
        internal O_CommandTransBase OCommandTransBase;

        public EntityCropper(T entity, Curve boundary, WhichSideToKeep whichSideToKeep,
            O_CommandTransBase oCommandTransBase)
        {
            _entity = entity;
            _boundary = boundary;
            _whichSideToKeep = whichSideToKeep;
            _whichSideToKeepString = whichSideToKeep.ToString();
            OCommandTransBase = oCommandTransBase;
        }

        public virtual IEnumerable<ObjectId> Crop()
        {
            _ptIntersects = _boundary.IntersectWith(_entity);
            //            if(!(_entity is BlockReference))_commandTransBase.AddPointsToBlockTableRecord(_ptIntersects,_entity.BlockId);
            IEnumerable<ObjectId> result = new List<ObjectId>();
            if (_ptIntersects.Count == 0)
                return result.Concat(CheckPosition());
            return result.Concat(Trim());
        }

        public static EntityCropperInterface<T> NewEntityCropper(T entity, Curve boundary,
            WhichSideToKeep whichSideToKeep,
            O_CommandTransBase oCommandTransBase)
        {
            if (entity is BlockReference)
                return new BlockReferenceCropper(entity as BlockReference, boundary, whichSideToKeep, oCommandTransBase)
                    as EntityCropperInterface<T>;
            if (entity is Curve)
                return new CurveCropper(entity as Curve, boundary, whichSideToKeep, oCommandTransBase) as
                    EntityCropperInterface<T>;
            if (entity is DBPoint)
                return new DbPointCropper(entity as DBPoint, boundary, whichSideToKeep, oCommandTransBase) as
                    EntityCropperInterface<T>;
            if (entity is DBText)
                return new DbTextCropper(entity as DBText, boundary, whichSideToKeep, oCommandTransBase) as
                    EntityCropperInterface<T>;
            if (entity is Dimension)
                return new DimCropper(entity as Dimension, boundary, whichSideToKeep, oCommandTransBase) as
                    EntityCropperInterface<T>;
            if (entity is Face)
                return new FaceCropper(entity as Face, boundary, whichSideToKeep, oCommandTransBase) as
                    EntityCropperInterface<T>;
            if (entity is Hatch)
                return new HatchCropper(entity as Hatch, boundary, whichSideToKeep, oCommandTransBase) as
                    EntityCropperInterface<T>;
            if (entity is MLeader)
                return new MLeaderCropper(entity as MLeader, boundary, whichSideToKeep, oCommandTransBase) as
                    EntityCropperInterface<T>;
            if (entity is Mline)
                return new MLineCropper(entity as Mline, boundary, whichSideToKeep, oCommandTransBase) as
                    EntityCropperInterface<T>;
            if (entity is MText)
                return new MTextCropper(entity as MText, boundary, whichSideToKeep, oCommandTransBase) as
                    EntityCropperInterface<T>;
            if (entity is Region)
                return new RegionCropper(entity as Region, boundary, whichSideToKeep, oCommandTransBase) as
                    EntityCropperInterface<T>;
            if (entity is Shape)
                return new ShapeCropper(entity as Shape, boundary, whichSideToKeep, oCommandTransBase) as
                    EntityCropperInterface<T>;
            if (entity is Solid)
                return new SolidCropper(entity as Solid, boundary, whichSideToKeep, oCommandTransBase) as
                    EntityCropperInterface<T>;
            return new EntityCropper<T>(entity, boundary, whichSideToKeep, oCommandTransBase);
        }

        internal virtual IEnumerable<ObjectId> Trim()
        {
            return new List<ObjectId>();
        }

        internal IEnumerable<ObjectId> CheckPosition()
        {
            var containmentString = _boundary.GetPointContainment(GetPosition()).ToString();

            if (containmentString == _whichSideToKeepString) return new List<ObjectId> { _entity.Id };
            OCommandTransBase.GetObjectForWrite(_entity.Id);
            _entity.Erase();
            return new List<ObjectId>();
        }

        internal virtual Point3d GetPosition()
        {
            return _entity.GetGeoExtentsCen();
        }
    }
}