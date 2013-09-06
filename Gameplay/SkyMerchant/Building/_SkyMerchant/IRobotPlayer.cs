using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Building._SkyMerchant
{
    /// <summary>
    /// Responsible for the robot player
    /// Implemented by the engine
    /// </summary>
    public interface IRobotPlayer
    {
        void PlaceSelectItemAt(Vector3 pos);
        void Pickup(IItem item);
        IIsland Island { get; }

        Ray TargetingRay { get; }
    }

    [ModelObjectChanged]
    public class SimpleRobotPlayer : EngineModelObject, IRobotPlayer
    {
        public SimpleRobotPlayer(IIsland island)
        {
            Island = island;
        }
        /// <summary>
        /// Set this to the item the user has selected.
        /// This is also automatically set to the 
        /// </summary>
        public SimpleItem SelectedItem { get; set; }
        public void PlaceSelectItemAt(Vector3 pos)
        {
            SelectedItem.Physical.WorldMatrix = Matrix.Translation(pos);
            SelectedItem.Physical.Visible = true;

            SelectedItem = null;
        }

        public void Pickup(IItem item)
        {
            if (SelectedItem != null) TW.Data.RemoveObject(SelectedItem);
            SelectedItem = (SimpleItem)item;
            SelectedItem.Physical.Visible = false;
        }

        public IIsland Island { get; private set; }
        public Ray TargetingRay
        { get { return TW.Data.Get<CameraInfo>().GetCenterScreenRay(); } }
    }
}