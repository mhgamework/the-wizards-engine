using System;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.SkyMerchant.Gameplay.Items
{
    /// <summary>
    /// Allows connecting islands with bridges
    /// </summary>
    public class BridgeToolItem : IHotbarItem
    {
        private readonly ILocalPlayer player;
        private readonly Func<BridgePart> createBridge;
        public string Name { get { return "Bridge Tool"; } }

        public BridgeToolItem(ILocalPlayer player, Func<BridgePart> createBridge)
        {
            this.player = player;
            this.createBridge = createBridge;
        }

        private BridgePart.BridgeAnchor startAnchor;
        private BridgePart.BridgeAnchor endAnchor;

        public void OnSelected()
        {
        }

        public void OnDeselected()
        {
        }

        public void Update()
        {
            if (TW.Graphics.Mouse.LeftMouseJustPressed) setStartPoint();
            if (TW.Graphics.Mouse.RightMouseJustPressed) { setEndPoint(); tryBuildBridge(); }
        }

        private void tryBuildBridge()
        {
            if (startAnchor.IsEmpty || endAnchor.IsEmpty) return;
            var b = createBridge();
            b.AnchorA = startAnchor;
            b.AnchorB = endAnchor;
        }

        private void setEndPoint()
        {
            endAnchor = getUserBridgeAnchor();
        }

        private void setStartPoint()
        {
            startAnchor = getUserBridgeAnchor();
        }

        private BridgePart.BridgeAnchor getUserBridgeAnchor()
        {
            if (!player.TargetedObjects.Any()) return new BridgePart.BridgeAnchor();
            var ret = new BridgePart.BridgeAnchor();

            ret.Island = player.TargetedObjects.First();
            ret.RelativePosition = player.GetPointTargetedOnObject(ret.Island).Value - ret.Island.Position;

            return ret;
        }

    }
}