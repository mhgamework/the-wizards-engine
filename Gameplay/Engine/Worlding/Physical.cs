using System;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using SlimDX;

namespace MHGameWork.TheWizards.Engine.Worlding
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
        public virtual bool Visible { get; set; }

        public virtual void Update()
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
            if (this.Mesh == null) return new BoundingBox(GetPosition(), GetPosition());
            if (Entity == null) return TW.Assets.GetBoundingBox(Mesh).Transform(ObjectMatrix * WorldMatrix);
            return TW.Assets.GetBoundingBox(Mesh).Transform(Entity.WorldMatrix);
        }

        /// <summary>
        /// Overrides previous worldmatrix settings!
        /// </summary>
        /// <param name="pos"></param>
        public void SetPosition(Vector3 pos)
        {
            WorldMatrix = Matrix.Translation(pos);

        }

        public void MoveTo(Vector3 target, float speed, float elapsed)
        {
            var newPos = GetPosition();
            newPos = newPos + Vector3.Normalize(target - newPos) * speed * elapsed;
            SetPosition(newPos);
        }
    }
}