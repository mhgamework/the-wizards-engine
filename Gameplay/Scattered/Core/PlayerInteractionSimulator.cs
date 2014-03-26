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

            var trans = TW.Graphics.LineManager3D.WorldMatrix;
            TW.Graphics.LineManager3D.WorldMatrix = target.Node.Absolute;
            TW.Graphics.LineManager3D.AddBox(TW.Assets.GetBoundingBox(target.Entity.Mesh), new Color4(1, 1, 0));
            
            TW.Graphics.LineManager3D.WorldMatrix = trans;


            if (TW.Graphics.Keyboard.IsKeyPressed(Key.F))
            {
                target.Interact();
            }
        }
    }
}