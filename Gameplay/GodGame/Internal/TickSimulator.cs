namespace MHGameWork.TheWizards.GodGame.Internal
{
    public class TickSimulator : ITickSimulator
    {
        private readonly World world;
        private IVoxelHandle handle;

        public const float TickInterval = 1 / 20f;
        private float nextTick;

        public TickSimulator(World world)
        {
            this.world = world;

            handle = new IVoxelHandle(world);

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