using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1;
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

        private Entity entity;

        public IslandRenderData(Island island)
        {
            this.island = island;
            rect = new TextRectangle();
            if (entity == null) entity = new Entity();
            entity.Mesh = UtilityMeshes.CreateBoxWithTexture(null, new Vector3(3));
        }


        public void UpdateRenderState()
        {
            rect.Text = island.Construction.Name + "\n" + island.Inventory.CreateItemsString();
            rect.Position = island.Position + Vector3.UnitY * 4;
            rect.Normal = Vector3.UnitY;
            rect.Radius = 1;
            rect.IsBillboard = true;
            //rect.PreTransformation = Matrix.RotationY(island.RotationY);
            rect.Update();


            entity.WorldMatrix = Matrix.RotationY(island.RotationY)*Matrix.Translation(island.Position);

        }

        public void Dispose()
        {
            if (rect != null)
                rect.Dispose();
            rect = null;
        }

        public Entity GetEntity()
        {
            return entity;
        }
    }
}