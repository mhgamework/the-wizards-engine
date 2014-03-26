using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class Enemy : IIslandAddon
    {
        public SceneGraphNode Node { get; private set; }
        public void PrepareForRendering()
        {

        }

        public Enemy(Level level, SceneGraphNode node)
        {
            Node = node;
            node.AssociatedObject = this;
            node.AssociatedObject = this;
            var ent = level.CreateEntityNode(node.CreateChild());
            ent.Entity.Mesh = TW.Assets.LoadMesh("Scattered\\Models\\EnemyRobot");
            ent.Node.Relative = Matrix.Scaling(3, 3, 3) * Matrix.Translation(0, 5, 0);
        }
    }
}