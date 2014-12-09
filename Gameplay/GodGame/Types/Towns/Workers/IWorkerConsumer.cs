namespace MHGameWork.TheWizards.GodGame.Types.Towns.Workers
{
    public interface IWorkerConsumer
    {
        int RequestedWorkersCount { get; }

        int AllocatedWorkersCount { get; }
        void AllocateWorkers(int amount);

    }
}