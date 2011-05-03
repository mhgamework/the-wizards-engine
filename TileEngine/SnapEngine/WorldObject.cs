using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Graphics;

namespace MHGameWork.TheWizards.TileEngine
{
    public class WorldObject : ICloneable, ISnappableWorldTarget
    {
        private IXNAGame game;

        private SimpleMeshRenderElement renderElement;
        public SimpleMeshRenderer Renderer;

        
        private Vector3 position; //Position of the center of gravity (defined by the gizmo in 3dsmax)

        public Vector3 Position
        {
            get { return position; }
            set { position = value; updateWorldMatrix(); }
        }
        private Quaternion rotation = Quaternion.Identity;

        public Quaternion Rotation
        {
            get { return rotation; }
            set { rotation = value; updateWorldMatrix(); }
        }
        public WorldObjectType ObjectType;

        private bool isDeleted;

        public bool IsDeleted
        {
            get { return isDeleted; }
            set { isDeleted = value; updateWorldMatrix(); }
        }
        

        private Matrix worldMatrix = Matrix.Identity;
        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            private set { worldMatrix = value; }
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

       

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return Clone();
        }

        public WorldObject Clone()
        {
            return new WorldObject(game, ObjectType, Renderer);
        }


        #endregion



        #region ISnappableWorldTarget Members

        public SnapInformation SnapInformation
        {
            get { return ObjectType.SnapInformation; }
        }

        public Transformation Transformation
        {
            get { return new Transformation(Vector3.One,rotation, position); }
        }

        #endregion
    }
}
