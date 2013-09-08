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
        public const float LineLodDistance = 30;
        public override void Update()
        {
            base.Update();
            var meshVisible = Vector3.Distance(TW.Data.Get<CameraInfo>().ActiveCamera.ViewInverse.xna().Translation.dx(), GetPosition()) <
                               LineLodDistance;

            Entity.Visible = meshVisible;
            if (!meshVisible)
                TW.Graphics.LineManager3D.AddAABB(GetBoundingBox(), Matrix.Identity, new Color4(0, 0, 0));

        }
        public override bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }
    }
}