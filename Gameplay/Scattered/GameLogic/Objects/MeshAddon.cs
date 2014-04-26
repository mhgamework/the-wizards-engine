using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Bindings;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;

namespace MHGameWork.TheWizards.Scattered.GameLogic.Objects
{
    public class MeshAddon : IIslandAddon
    {
        private readonly IMesh mesh;
        private readonly Level level;

        public MeshAddon(IMesh mesh, Level level, SceneGraphNode node)
        {
            this.mesh = mesh;
            this.level = level;
            Node = node;
            Node.AssociatedObject = this;

            var ent = level.CreateEntityNode(node);
            ent.Entity.Mesh = mesh;
        }

        public SceneGraphNode Node { get; private set; }
        public void PrepareForRendering()
        {
            //nothing to do
        }
    }
}
