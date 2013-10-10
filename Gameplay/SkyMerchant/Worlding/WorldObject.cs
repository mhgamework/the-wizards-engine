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
    public class WorldObject : EngineModelObject, IWorldObject
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

    }
}