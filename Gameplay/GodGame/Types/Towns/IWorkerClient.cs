namespace MHGameWork.TheWizards.GodGame.Types.Towns
{
    public interface IWorkerClient
    {
        int MaxWorkers { get; set; }
        int AssignedWorkers { set; }
    }
}