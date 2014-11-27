namespace MHGameWork.TheWizards.GodGame.Types.Workers
{
    public interface IWorkerConsumer
    {
        int RequestedWorkersCount { get; }

        int AllocatedWorkersCount { get; set; } //TODO: make set private
    }
}