using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered._Engine;
using SlimDX;
using IDisposable = System.IDisposable;

namespace MHGameWork.TheWizards.Scattered.Rendering
{
    public class IslandRenderData : IRenderable, IDisposable
    {
        private readonly Island island;
        private TextRectangle rect;

        public IslandRenderData(Island island)
        {
            this.island = island;
            rect = new TextRectangle();
        }


        public void UpdateRenderState()
        {
            rect.Text = island.Construction.Name + "\n" + island.Inventory.CreateItemsString();
            rect.Position = island.Position;
            rect.Normal = Vector3.UnitY;
            rect.Radius = 5;

            rect.Update();

        }

        public void Dispose()
        {
            if (rect != null)
                rect.Dispose();
            rect = null;
        }
    }
}