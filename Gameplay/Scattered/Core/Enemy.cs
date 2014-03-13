using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class Enemy : IIslandAddon
    {
        public SceneGraphNode Node { get; private set; }

        public Enemy(Level level, SceneGraphNode node)
        {
            Node = node;
            var ent = level.CreateEntityNode(node.CreateChild());
            ent.Entity.Mesh = TW.Assets.LoadMesh("Scattered\\Models\\EnemyRobot");
        }
    }
}