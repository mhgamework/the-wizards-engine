using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types.Towns.Workers;

namespace MHGameWork.TheWizards.GodGame.Types.Towns
{
    public interface IIndustryBuildingType
    {
        IWorkerConsumer GetWorkerConsumer(IVoxelHandle handle);
    }
}