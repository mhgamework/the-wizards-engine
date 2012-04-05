using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Building;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.Model
{
    public class TiledEntity : BaseModelObject
    {
        public static readonly Vector3 TileSize = new Vector3(3, 4, 3);

        private Entity entity;

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
            return Matrix.RotationY(getRotationRadians(tileRotation)) * Matrix.Translation(new Vector3(position.X * TileSize.X, position.Y * TileSize.Y, position.Z * TileSize.Z) + TileSize * 0.5f);
        }

        private static float getRotationRadians(TileRotation rot)
        {
            switch (rot)
            {
                case TileRotation.Rotation0:
                    return 0;
                case TileRotation.Rotation180:
                    return MathHelper.Pi;
                case TileRotation.Rotation90:
                    return MathHelper.PiOver2;
                case TileRotation.Rotation270:
                    return MathHelper.PiOver2 * 3;

            }
            throw new InvalidOperationException();
        }

        public TiledEntity()
        {
            entity = new Entity();

        }



    }
}
