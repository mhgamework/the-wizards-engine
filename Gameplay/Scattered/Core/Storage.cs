using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class Storage : IIslandAddon
    {
        public Storage(Level level, SceneGraphNode node)
        {
        }

        public SceneGraphNode Node { get; private set; }
    }
}