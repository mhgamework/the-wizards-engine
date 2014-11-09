using System.Collections.Generic;

namespace MHGameWork.TheWizards.GodGame.Types.Workers
{
    public interface ITown
    {
        IEnumerable<WorkerProducer> Producers { get; }
        IEnumerable<WorkerConsumer> Consumers { get; }
    }
}