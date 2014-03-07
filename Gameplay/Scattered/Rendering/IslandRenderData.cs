using System.Collections.Generic;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;
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

        //TODO share this
        private Dictionary<Island.IslandType, IMesh> meshes = new Dictionary<Island.IslandType, IMesh>();

        public IslandRenderData(Island island)
        {
            this.island = island;
            rect = new TextRectangle();
            if (entity == null) entity = new Entity();

            meshes.Add(Island.IslandType.Normal, TW.Assets.LoadMesh("Scattered\\Models\\Island_Medium"));
            meshes.Add(Island.IslandType.Resource, TW.Assets.LoadMesh("Scattered\\Models\\Island_Small"));
            meshes.Add(Island.IslandType.Tower, TW.Assets.LoadMesh("Scattered\\Models\\Island_Large"));

            //entity.Mesh = TW.Assets.LoadMesh("Scattered\\Models\\Island_Medium");// UtilityMeshes.CreateBoxWithTexture(null, new Vector3(3));
        }


        public void UpdateRenderState()
        {
            rect.Text = island.Construction.Name + "\n" + island.Inventory.CreateItemsString();
            rect.Position = island.Position + Vector3.UnitY * 1;
            rect.Normal = Vector3.UnitY;
            rect.Radius = 0.7f;
            rect.IsBillboard = true;
            //rect.PreTransformation = Matrix.RotationY(island.RotationY);
            rect.Update();

            entity.Mesh = meshes[island.Type];
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