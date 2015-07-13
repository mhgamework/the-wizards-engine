using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;
using MHGameWork.TheWizards.SkyMerchant._Engine.Spatial;

namespace MHGameWork.TheWizards.SkyMerchant.Gameplay
{
    /// <summary>
    /// Works by picking up objects and placing them in a fixed position relative to the camera, depending on their size.
    /// Note: probably a bad idea, better use something that uses the pickup distance as the relative position.
    /// </summary>
    public class WorldObjectMover
    {
        private readonly ICamera cam;

        private IMutableSpatial holdingItem;

        public IMutableSpatial HoldingItem
        {
            get { return holdingItem; }
        }

        public WorldObjectMover(ICamera cam)
        {
            this.cam = cam;
        }

        public void Pickup(IMutableSpatial item)
        {
            Drop();
            holdingItem = item;


        }

        public void Drop()
        {
            holdingItem = null;
        }

        public void Update()
        {
            if (holdingItem == null) return;
            var camPos = cam.ViewInverse.xna().Translation.dx();
            var camDir = cam.ViewInverse.xna().Forward.dx();

            var dist = 10f;

            dist = (holdingItem.LocalBoundingBox.Maximum - holdingItem.LocalBoundingBox.Minimum).MaxComponent() * 4;

            holdingItem.Position = camPos + camDir * dist;
        }

    }
}