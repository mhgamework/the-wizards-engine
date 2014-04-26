using MHGameWork.TheWizards.Scattered.SceneGraphing;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public interface IIslandAddon : IHasNode
    {
        void PrepareForRendering();
    }
}