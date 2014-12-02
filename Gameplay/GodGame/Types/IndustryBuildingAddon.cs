using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types.Towns.Workers;

namespace MHGameWork.TheWizards.GodGame.Types
{
    /// <summary>
    /// Simulates common industry building behaviour for a single voxel
    /// Represents an addon object which is associated with each individual voxel that represents an industry building
    /// </summary>
    public class IndustryBuildingAddon : VoxelInstanceAddon
    {
        public IWorkerConsumer WorkerConsumer { get; private set; }
        public override void OnCreated(IVoxelHandle handle)
        {
            WorkerConsumer = new SimpleWorkerConsumer() { RequestedWorkersCount = 5 };
        }

        public override void OnDestroyed(IVoxelHandle handle)
        {
            WorkerConsumer = null;
        }
    }
}