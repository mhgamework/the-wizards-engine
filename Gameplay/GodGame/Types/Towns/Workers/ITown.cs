using System.Collections.Generic;

namespace MHGameWork.TheWizards.GodGame.Types.Towns.Workers
{
    public interface ITown
    {
        IEnumerable<IWorkerProducer> Producers { get; }
        IEnumerable<IWorkerConsumer> Consumers { get; }
    }
}