namespace MHGameWork.TheWizards.GodGame.Types.Workers
{
    public class WorkerConsumer
    {
        public int RequestedWorkersCount { get; set; }

        public int AllocatedWorkersCount { get; set; } //TODO: make set private
    }
}