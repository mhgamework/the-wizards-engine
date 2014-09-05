using DirectX11;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    public class PerEntityVoxelWorldRenderer : IVoxelWorldRenderer
    {
        private readonly World world;

        public PerEntityVoxelWorldRenderer(World world, Point2 windowSize)
        {
            this.world = world;
            entities = new Array2D<Entity>(windowSize);
            entities.ForEach((e, p) => entities[p] = new Entity());
        }

        private Array2D<Entity> entities;
        public void UpdateWindow(Point2 offset, Vector3 worldTranslation, Point2 windowSize)
        {
            entities.ForEach((e, p) =>
            {
                var v = world.GetVoxel(p + new Point2(offset));
                if (v == null)
                {
                    e.Visible = false;
                    return;
                }
                e.Mesh = getMesh(v);
                e.WorldMatrix = Matrix.Scaling(new Vector3(world.VoxelSize.X)) *
                                Matrix.Translation(world.GetBoundingBox(p).GetCenter() + worldTranslation);
                e.Visible = e.Mesh != null;
            });
        }

        private IMesh getMesh(GameVoxel gameVoxel)
        {
            if (gameVoxel.Type == null) return null;

            var handle = new IVoxelHandle(world, gameVoxel);

            return gameVoxel.Type.GetMesh(handle);

        }

    }
}