using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.GodGame
{
    public class TickSimulator : ISimulator
    {
        private readonly World world;
        private ITickHandle handle;

        public const float TickInterval = 1 / 20f;
        private float nextTick;

        public TickSimulator(World world)
        {
            this.world = world;

            handle = new ITickHandle(world);

            nextTick = TW.Graphics.TotalRunTime + TickInterval;
        }

        public void Simulate()
        {
            if (nextTick > TW.Graphics.TotalRunTime) return;
            nextTick += TickInterval; //TODO: Check for timing problems
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