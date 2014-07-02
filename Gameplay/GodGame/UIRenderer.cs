using System.Drawing;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.GodGame._Tests;
using MHGameWork.TheWizards.Scattered.GameLogic.Services;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame
{
    /// <summary>
    /// TODO: targetingreticle not working
    /// </summary>
    public class UIRenderer : ISimulator
    {
        private readonly World world;
        private readonly PlayerInputSimulator inputSim;
        private TargetingReticle reticle;
        private Textarea textarea;

        public UIRenderer(World world, PlayerInputSimulator inputSim)
        {
            this.world = world;
            this.inputSim = inputSim;

            reticle = new TargetingReticle();

            textarea = new Textarea();
            textarea.Position = new Vector2(TW.Graphics.Form.Form.ClientSize.Width - 120, 20);
            textarea.Size = new Vector2(100, 50);
            
        }

        public void Simulate()
        {
            updateTextarea();
            reticle.drawReticle();
            drawSelectedVoxel();
            drawWorldBoundingbox();
        }

        private void updateTextarea()
        {
            textarea.Text = inputSim.ActiveHandler.Name;
        }

        private void drawWorldBoundingbox()
        {
            TW.Graphics.LineManager3D.AddBox(new BoundingBox(new Vector3(0, 0, 0), new Vector3(world.VoxelSize.X * world.WorldSize, 1, world.VoxelSize.Y * world.WorldSize)), new Color4(0, 0, 0));
        }

        private void drawSelectedVoxel()
        {
            var v = inputSim.GetTargetedVoxel();
            if (v == null) return;
            var bb = v.GetBoundingBox();
            bb.Maximum.Y = 1f;

            TW.Graphics.LineManager3D.AddBox(bb, Color.Yellow.dx());
        }
    }
}