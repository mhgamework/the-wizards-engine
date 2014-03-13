using System;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered._Engine;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class PlayerInteractionSimulator : ISimulator
    {
        private readonly Level level;
        private readonly ScatteredPlayer player;

        public PlayerInteractionSimulator(Level level, ScatteredPlayer player)
        {
            this.level = level;
            this.player = player;
        }

        public void Simulate()
        {
            var target = level.InteractableNodes.Raycast((n, r) => n.Intersects(r), TW.Data.Get<CameraInfo>().GetCenterScreenRay());

            if (target == null) return;

            if (target.Entity.Mesh == null) throw new InvalidOperationException("This is magic!");

            TW.Graphics.LineManager3D.AddAABB(TW.Assets.GetBoundingBox(target.Entity.Mesh), target.Node.Absolute, new Color4(1, 1, 0));


            if (TW.Graphics.Keyboard.IsKeyPressed(Key.F))
            {
                target.Interact();
            }
        }
    }
}