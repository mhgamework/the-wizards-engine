using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types.Towns.Workers;

namespace MHGameWork.TheWizards.GodGame.Types
{
    /// <summary>
    /// Simulates common industry building behaviour for a single voxel
    /// Represents an addon object which is associated with each individual voxel that represents an industry building
    /// </summary>
    public class IndustryBuildingAddon : VoxelInstanceAddon, IWorkerConsumer
    {
        public IndustryBuildingAddon()
        {
            RequestedWorkersCount = 1;
        }
        public override void OnCreated(IVoxel handle)
        {
        }

        public override void OnDestroyed(IVoxel handle)
        {
        }
        public override string GetDebugDescription()
        {
            return string.Format("Workers: {0}/{1}",
                this.AllocatedWorkersCount,
                                 RequestedWorkersCount);
        }

        public int RequestedWorkersCount { get; set; }
        public int AllocatedWorkersCount { get; private set; }
        public void AllocateWorkers(int amount)
        {
            AllocatedWorkersCount = amount;
        }
    }
}