using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Engine.Worlding;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Lod
{
    /// <summary>
    /// Responsible for physically representing an object in the world
    /// When the object is at a certain distance form the viewer it is shown as a boundingbox
    /// </summary>
    public class LineLodPhysical : Physical
    {
        private bool visible;
        private CameraInfo info;
        public const float LineLodDistance = 60;


        public LineLodPhysical()
        {
            info = TW.Data.Get<CameraInfo>();
        }

        public override void Update()
        {
            base.Update();
            UpdateMeshVisibility();
        }

        public void UpdateMeshVisibility()
        {
            if (Entity == null) return;
            var meshVisible =
                Vector3.Distance(info.ActiveCamera.ViewInverse.xna().Translation.dx(), GetPosition()) <
                LineLodDistance;

            if (Entity.Visible != meshVisible)
                Entity.Visible = meshVisible;
            if (!meshVisible) ;
            TW.Graphics.LineManager3D.AddAABB(GetBoundingBox(), Matrix.Identity, new Color4(0, 0, 0));
        }

        public override bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }
    }
}