using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered._Engine;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.Rendering
{
    public class IslandRenderData : IRenderable
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
            rect.Radius = 10;

            rect.Update();

        }
    }
}