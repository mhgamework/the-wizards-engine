using MHGameWork.TheWizards.DualContouring.Rendering;

namespace MHGameWork.TheWizards.DualContouring.Building
{
    public class FiniteWorldRenderer
    {
        private readonly FiniteWorld world;
        private readonly VoxelCustomRenderer renderer;

        public FiniteWorldRenderer(FiniteWorld world, VoxelCustomRenderer renderer)
        {
            this.world = world;
            this.renderer = renderer;
        }
        public void UpdateRenderer()
        {
            world.Chunks.ForEach((c, p) =>
                {
                    if (c.SurfaceDirty) c.UpdateSurface(renderer);
                });
        }
    }
}