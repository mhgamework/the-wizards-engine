using System.Drawing;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.GodGame._Tests;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame
{
    public class UIRenderer : ISimulator
    {
        private readonly World world;
        private readonly PlayerInputSimulator inputSim;

        public UIRenderer(World world, PlayerInputSimulator inputSim)
        {
            this.world = world;
            this.inputSim = inputSim;
        }

        public void Simulate()
        {
            var v = inputSim.GetTargetedVoxel();
            if (v == null) return;
            var bb = v.GetBoundingBox();
            bb.Maximum.Y = 1f;

            TW.Graphics.LineManager3D.AddBox(bb, Color.Yellow.dx());
        }
    }
}