using System.Diagnostics.Contracts;
using System.Drawing;
using MHGameWork.TheWizards.Engine;
using Castle.Core.Internal;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.Rendering
{
    [ContractVerification(true)]
    public class LevelRenderer : ISimulator
    {
        private readonly Level level;

        public LevelRenderer(Level level)
        {
            this.level = level;
        }

        public void Simulate()
        {
            level.Islands.ForEach(drawIsland);
            level.Travellers.ForEach(drawTraveller);
        }

        private void drawTraveller(Traveller obj)
        {
            TW.Graphics.LineManager3D.AddCenteredBox(obj.BridgePosition.CalculateActualPositon(), 1f, new Color4(0, 1, 0));
        }

        private void drawIsland(Island obj)
        {
            obj.RenderData.UpdateRenderState();

            obj.ConnectedIslands.ForEach(i2 => TW.Graphics.LineManager3D.AddLine(obj.Position, i2.Position, new Color4(Color.SaddleBrown)));

        }



    }
}