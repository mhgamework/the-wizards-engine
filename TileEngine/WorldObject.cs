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
        public WorldObjectType ObjectType;

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
                float yaw, roll, pitch;
                convertQuaternionToEuler(value, out yaw, out roll, out pitch);


                Vector3 t = Vector3.Transform(Vector3.Forward, value);

                if (Vector3.Dot(t, Vector3.Forward) > 0.99f)
                    rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, 0);
                else if (Vector3.Dot(t, Vector3.Backward) > 0.99f)
                    rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi);
                else if (Vector3.Dot(t, Vector3.Left) > 0.99f)
                    rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver2);
                else if (Vector3.Dot(t, Vector3.Right) > 0.99f)
                    rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, -MathHelper.PiOver2);
                else
                {
                    throw new Exception();
                }

                //rotation = value; 
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

        private void convertQuaternionToEuler(Quaternion quat, out float yaw, out float roll, out float pitch)
        {
            yaw = (float)Math.Atan2(2 * (quat.X * quat.Y + quat.Z * quat.W), 1 - 2 * (quat.Y * quat.Y + quat.Z * quat.Z));
            pitch = (float)Math.Asin(2 * (quat.X * quat.Z - quat.W * quat.Y));
            roll = (float)Math.Atan2(2 * (quat.X * quat.W + quat.Y * quat.Z), 1 - 2 * (quat.Z * quat.Z + quat.W * quat.W));

#if DEBUG

            Quaternion verify = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
            Vector3 up1 = Vector3.Transform(Vector3.Up, quat);
            Vector3 up2 = Vector3.Transform(Vector3.Up, verify);
            Vector3 right1 = Vector3.Transform(Vector3.Right, quat);
            Vector3 right2 = Vector3.Transform(Vector3.Right, verify);

            //TODO: Testen
#endif

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
