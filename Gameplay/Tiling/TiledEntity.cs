using System;
using DirectX11;
using MHGameWork.TheWizards.Building;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.Tiling
{
    public class TiledEntity : BaseModelObject
    {
        public static readonly Vector3 TileSize = new Vector3(3, 4, 3);

        private WorldRendering.Entity entity;

        private Point3 position;
        public Point3 Position
        {
            get { return position; }
            set
            {
                position = value;
                updateEntity();
            }
        }

        private TileRotation rotation;
        public TileRotation Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                updateEntity();
            }
        }

        public IMesh Mesh
        {
            get { return entity.Mesh; }
            set
            {
                entity.Mesh = value;
            }
        }

        private void updateEntity()
        {
            entity.WorldMatrix = CreateEntityWorldMatrix(rotation, position);
        }

        public static Matrix CreateEntityWorldMatrix(TileRotation tileRotation, Point3 position)
        {
            return Matrix.RotationY(tileRotation.GetRadians()) * Matrix.Translation(new Vector3(position.X * TileSize.X, position.Y * TileSize.Y, position.Z * TileSize.Z) + TileSize * 0.5f);
        }

        public TiledEntity()
        {
            entity = new WorldRendering.Entity();

        }

        /// <summary>
        /// TODO: design generic pattern for disposal!
        /// </summary>
        public void Delete()
        {
            TW.Model.RemoveObject(entity);
            TW.Model.RemoveObject(this);
        }



    }
}
