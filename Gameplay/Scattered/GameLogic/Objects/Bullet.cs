using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.GameLogic.Objects
{
    public class Bullet
    {
        private Level level;
        public SceneGraphNode Node { get; private set; }
        public Vector3 Position { get; private set; }
        private Vector3 direction;
        private readonly float speed;
        private float lifetime;

        public Bullet(Level level, SceneGraphNode node, Vector3 position, Vector3 direction, float speed, float lifetime)
        {
            this.level = level;
            Node = node;
            node.Absolute = Matrix.Translation(position);
            Position = position;
            this.direction = direction;
            this.direction.Normalize();
            this.speed = speed;
            this.lifetime = lifetime;

            var ent = level.CreateEntityNode(node.CreateChild());
            ent.Entity.Mesh = TW.Assets.LoadMesh("Scattered\\Models\\Bullet");
            ent.Node.Relative = Matrix.Scaling(2, 2, 2);

            level.AddBehaviour(node,Update);

        }

        public void Update()
        {
            if (level == null) return;
            Position += direction * speed * TW.Graphics.Elapsed;
            Node.Absolute = Matrix.Translation(Position);

            lifetime -= TW.Graphics.Elapsed;
            if (lifetime < 0)
            {
                Dispose();
                return;
            }

            if (Vector3.Distance(level.LocalPlayer.Position, Position) < 0.5f)
            {
                level.LocalPlayer.TakeDamage(0.2f);
                Dispose();
            }

        }

        public void Dispose()
        {
            if (level == null) return;
            level.DestroyNode(Node);
            level = null;
        }
    }
}
