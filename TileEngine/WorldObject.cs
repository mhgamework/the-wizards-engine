using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.TileEngine.SnapEngine;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Graphics;

namespace MHGameWork.TheWizards.TileEngine
{
    public class WorldObject : ISnappableWorldTarget
    {
        public WorldObjectType ObjectType { get; set; }

        private IXNAGame game;
        private SimpleMeshRenderElement renderElement;
        public SimpleMeshRenderer Renderer;


        private Vector3 position;
        public Vector3 Position
        {
            get { return position; }
            set { position = value; updateWorldMatrix(); }
        }

        private Quaternion rotation = Quaternion.Identity;
        public Quaternion Rotation
        {
            get { return rotation; }
            set
            {
                

                //Vector3 t = Vector3.Transform(Vector3.Forward, value);

                //if (Vector3.Dot(t, Vector3.Forward) > 0.99f)
                //    rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, 0);
                //else if (Vector3.Dot(t, Vector3.Backward) > 0.99f)
                //    rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi);
                //else if (Vector3.Dot(t, Vector3.Left) > 0.99f)
                //    rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver2);
                //else if (Vector3.Dot(t, Vector3.Right) > 0.99f)
                //    rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, -MathHelper.PiOver2);
                //else
                //{
                //    throw new Exception();
                //}

                rotation = value; 
                updateWorldMatrix();
            }
        }

        

        private Matrix worldMatrix = Matrix.Identity;
        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            private set { worldMatrix = value; }
        }

        private bool isDeleted;
        public bool IsDeleted
        {
            get { return isDeleted; }
            set { isDeleted = value; updateWorldMatrix(); }
        }



        public WorldObject(IXNAGame game, WorldObjectType objectType, SimpleMeshRenderer renderer)
        {
            this.game = game;
            ObjectType = objectType;
            Renderer = renderer;
            renderElement = Renderer.AddMesh(ObjectType.Mesh);
            updateWorldMatrix();
        }

        private void updateWorldMatrix()
        {
            if (IsDeleted)
                worldMatrix = new Matrix();
            else
                WorldMatrix = Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);

            renderElement.WorldMatrix = ObjectType.TileData.MeshOffset * WorldMatrix;
        }

        public void Reset()
        {
            Position = new Vector3(0, 0, 0);
            Rotation = Quaternion.Identity;
        }

        

        #region ISnappableWorldTarget Members

        public SnapInformation SnapInformation
        {
            get { return ObjectType.SnapInformation; }
        }

        public Transformation Transformation
        {
            get { return new Transformation(Vector3.One, rotation, position); }
        }

        #endregion
    }
}
