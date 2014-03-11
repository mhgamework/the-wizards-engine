using System.Collections.Generic;
using DirectX11;
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

        private Dictionary<Island.BridgeConnector, Entity> bridges = new Dictionary<Island.BridgeConnector, Entity>();


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
            rect.Text =  island.Inventory.CreateItemsString();
            rect.Position = island.Position + Vector3.UnitY * 1;
            rect.Normal = Vector3.UnitY;
            rect.Radius = 0.7f;
            rect.IsBillboard = true;
            //rect.PreTransformation = Matrix.RotationY(island.RotationY);
            rect.Update();

            entity.Mesh = meshes[island.Type];
            entity.WorldMatrix = Matrix.RotationY(island.RotationY) * Matrix.Translation(island.Position);

            island.BridgeConnectors.ForEach(updateBridge);


        }

        private void updateBridge(Island.BridgeConnector bridgeConnector)
        {
            if (!bridges.ContainsKey(bridgeConnector))
            {
                var e = new Entity();
                e.Mesh = TW.Assets.LoadMesh("Scattered\\Models\\BridgeConnector");
                bridges[bridgeConnector] = e;
            }
            var ent = bridges[bridgeConnector];

            ent.WorldMatrix = Matrix.RotationY(MathHelper.PiOver2 * 2) *
                Matrix.Invert(Matrix.LookAtRH(bridgeConnector.RelativePosition,
                                              bridgeConnector.RelativePosition + bridgeConnector.Direction,
                                              Vector3.UnitY)) * island.GetWorldMatrix();
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