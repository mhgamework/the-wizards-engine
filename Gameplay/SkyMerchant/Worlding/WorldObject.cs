using System.Collections.Generic;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Worlding
{
    /// <summary>
    /// Implements the IWorldObject interface for use in the SkyMerchant project
    /// </summary>
    [ModelObjectChanged]
    public class WorldObject : EngineModelObject, IWorldObject, IPhysicalPart
    {
        private readonly Physical ph;

        public WorldObject(Physical ph)
        {
            this.ph = ph;
            Scripts = new WorldObjectScriptsList(this);
            Visible = true;
            Enabled = true;
        }


        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public bool Visible { get; set; }
        public bool Enabled { get; set; }

        /// <summary>
        /// Updates the state of the underlying physical
        /// </summary>
        public void UpdatePhysical()
        {
            ph.WorldMatrix = Matrix.RotationQuaternion(Rotation) * Matrix.Translation(Position);
            ph.Visible = Visible && Enabled;
        }
        public BoundingBox LocalBoundingBox
        { get { return ph.Mesh == null ? new BoundingBox() : TW.Assets.GetBoundingBox(ph.Mesh).Transform(ph.ObjectMatrix); } }

        public ICollection<IWorldScript> Scripts { get; private set; }



        Matrix IPhysicalPart.ObjectMatrix
        {
            get { return ph.ObjectMatrix; }
            set { ph.ObjectMatrix = value; }
        }

        Matrix IPhysicalPart.WorldMatrix
        {
            get { return this.GetWorldMatrix(); }
            set
            {
                Vector3 scale, translation;
                Quaternion rotation;
                value.Decompose(out scale, out rotation, out translation);
                //TODO: no scaling support!

                Position = translation;
                Rotation = rotation;
            }
        }

        bool IPhysicalPart.Solid
        {
            get { return ph.Solid; }
            set { ph.Solid = value; }
        }

        Rendering.IMesh IPhysicalPart.Mesh
        {
            get { return ph.Mesh; }
            set { ph.Mesh = value; }
        }

        bool IPhysicalPart.Static
        {
            get { return ph.Static; }
            set { ph.Static = value; }
        }

        void IPhysicalPart.Update()
        {
            ph.Update();
        }

        Vector3 IPhysicalPart.GetPosition()
        {
            return Position;
        }

        BoundingBox IPhysicalPart.GetBoundingBox()
        {
            return LocalBoundingBox.Transform(this.GetWorldMatrix());
        }

        void IPhysicalPart.SetPosition(Vector3 pos)
        {
            Position = pos;
        }

    }
}