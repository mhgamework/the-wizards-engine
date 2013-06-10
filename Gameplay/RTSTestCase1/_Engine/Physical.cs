using System;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1
{
    /// <summary>
    /// Should not be using an interface, should be an interface on its own.
    /// </summary>
    [ModelObjectChanged]
    public class Physical : EngineModelObject
    {

        public Physical()
        {
            WorldMatrix = Matrix.Identity;
            ObjectMatrix = Matrix.Identity;
            Visible = true;
        }

        /// <summary>
        /// Internal use!!
        /// </summary>
        public Entity Entity { get; set; }
        public Matrix ObjectMatrix { get; set; }
        public Matrix WorldMatrix { get; set; }

        public bool Solid { get; set; }
        public IMesh Mesh { get; set; }
        public bool Static { get; set; }
        public bool Visible { get; set; }

        public void Update()
        {
            if (Entity == null) Entity = new Entity();

            // Note: this could get messy for buggies
            Entity.WorldMatrix = ObjectMatrix * WorldMatrix;
            Entity.Solid = Solid;
            Entity.Mesh = Mesh;
            Entity.Static = Static;
            Entity.Visible = Visible;

            //TODO: add another simulator after the one that calls this update, 
            //  to allow for debug visualization!!?

        }

        /// <summary>
        /// TODO: make this for debug visualization? (use other simulator)
        /// </summary>
        public void UpdateDebug()
        {
            throw new NotImplementedException();
        }

        public Vector3 GetPosition()
        {
            return WorldMatrix.xna().Translation.dx();
        }

        public BoundingBox GetBoundingBox()
        {
            if (this.Mesh == null) return new BoundingBox(GetPosition(),GetPosition());
            return TW.Assets.GetBoundingBox(Mesh).Transform(Entity.WorldMatrix);
        }
    }
}