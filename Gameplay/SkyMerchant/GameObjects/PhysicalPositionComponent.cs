using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.GameObjects
{
    /// <summary>
    /// Uses Physical to implement the PositionComponent
    /// TODO: maybe invert this! Have something track changes to the position component, and set the Physical accordingly
    /// </summary>
    public class PhysicalPositionComponent : IPositionComponent
    {
        private readonly SkyPhysical ph;

        public PhysicalPositionComponent(SkyPhysical ph)
        {
            this.ph = ph;
        }

        public Vector3 Position { get { return ph.GetPosition(); } set { ph.SetPosition(value); } }
        public Quaternion Rotation
        {
            get
            {
                //WARNING: scale not supported
                Vector3 scale, translation;
                Quaternion rotation;
                ph.WorldMatrix.Decompose(out scale, out rotation, out translation);
                return rotation;
            }
            set { ph.WorldMatrix = Matrix.RotationQuaternion(value) * Matrix.Translation(ph.GetPosition()); }
        }

        public BoundingBox LocalBoundingBox
        { get { return ph.Mesh == null ? new BoundingBox() : TW.Assets.GetBoundingBox(ph.Mesh).Transform(ph.ObjectMatrix); } }

    }
}