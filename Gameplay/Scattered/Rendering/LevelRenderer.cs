using System.Diagnostics.Contracts;
using System.Drawing;
using MHGameWork.TheWizards.Engine;
using Castle.Core.Internal;
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
        }

        private void drawIsland(Island obj)
        {
            Contract.Assume(TW.Graphics != null);
            var size = new Vector3(20, 5, 20);

            TW.Graphics.LineManager3D.AddBox(new BoundingBox(obj.Position - size * 0.5f, obj.Position + size * 0.5f), new Color4(Color.Green));

            obj.ConnectedIslands.ForEach(i2 => TW.Graphics.LineManager3D.AddLine(obj.Position,i2.Position,new Color4(Color.SaddleBrown)));

        }


        
    }
}