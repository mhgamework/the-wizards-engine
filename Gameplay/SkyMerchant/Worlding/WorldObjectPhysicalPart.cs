using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Worlding
{
    /// <summary>
    /// Implements the physical part using the IWorldObject and Physical as core
    /// </summary>
    public class WorldObjectPhysicalPart:EngineModelObject, IPhysicalPart
    {
        private readonly IWorldObject obj;
        private readonly Physical ph;

        public WorldObjectPhysicalPart(IWorldObject obj, Physical ph)
        {
            this.obj = obj;
            this.ph = ph;
            
        }


        public Matrix ObjectMatrix
        {
            get { return ph.ObjectMatrix; }
            set { ph.ObjectMatrix = value; }
        }

        public bool Solid
        {
            get { return ph.Solid; }
            set { ph.Solid = value; }
        }

        public IMesh Mesh
        {
            get { return ph.Mesh; }
            set { ph.Mesh = value; }
        }

        public bool Static
        {
            get { return ph.Static; }
            set { ph.Static = value; }
        }

        public Matrix WorldMatrix
        {
            get { return obj.GetWorldMatrix(); }
            set
            {
                Vector3 scale, translation;
                Quaternion rotation;
                value.Decompose(out scale, out rotation, out translation);
                //TODO: no scaling support!

                obj.Position = translation;
                obj.Rotation = rotation;
            }
        }

        public bool Visible
        {
            get { return obj.Visible; }
            set { obj.Visible = value; }
        }
        public void Update()
        {
        }

        public Vector3 GetPosition()
        {
            return obj.Position;
        }

        public BoundingBox GetBoundingBox()
        {
            return obj.LocalBoundingBox.Transform(obj.GetWorldMatrix());
        }

        public void SetPosition(Vector3 pos)
        {
            obj.Position = pos;
        }
    }
}