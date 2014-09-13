using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Model;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    /// <summary>
    /// Service that allows simulating a voxel world
    /// </summary>
    public class WorldSimulationService : ISimulator
    {
        private readonly Model.World world;
        private readonly VoxelTypesFactory voxelTypesFactory;
        private IVoxelHandle handle;

        public const float TickInterval = 1 / 20f;
        private float nextTick;

        public WorldSimulationService(Model.World world,VoxelTypesFactory voxelTypesFactory)
        {
            this.world = world;
            this.voxelTypesFactory = voxelTypesFactory;

            handle = new IVoxelHandle(world);

            nextTick = TW.Graphics.TotalRunTime + TickInterval;
        }

        public void Simulate()
        {
            if (nextTick > TW.Graphics.TotalRunTime) return;
            nextTick += TickInterval; //TODO: Check for timing problems
            voxelTypesFactory.AllTypes.ForEach(t => t.PerFrameTick());
            world.ForEach((v, p) =>
                {
                    if (v.Type == null) return;

                    handle.CurrentVoxel = v;

                    v.Type.Tick(handle);

                    handle.CurrentVoxel = null;
                });
        }
    }
}