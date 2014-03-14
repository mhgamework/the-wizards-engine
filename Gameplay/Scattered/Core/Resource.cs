using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class Resource : IIslandAddon
    {
        public Resource(Level level, SceneGraphNode node)
        {
            
        }

        public SceneGraphNode Node { get; private set; }
        public void PrepareForRendering()
        {
            
        }
    }
}