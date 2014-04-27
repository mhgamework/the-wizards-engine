using MHGameWork.TheWizards.Scattered.Bindings;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.GameLogic.Objects
{
    public class Bullet
    {
        public SceneGraphNode Node { get; private set; }
        public Vector3 Position { get; private set; }
        private IObjectHandle handle;
        private Vector3 direction;
        private readonly float speed;
        private float lifetime;

        public delegate Bullet Factory(Vector3 position, Vector3 direction, float speed, float lifetime);

        public Bullet( IObjectHandle handle, SceneGraphNode node, Vector3 position, Vector3 direction, float speed, float lifetime)
        {
            Node = node;
            node.Absolute = Matrix.Translation(position);
            Position = position;
            this.handle = handle;
            this.direction = direction;
            this.direction.Normalize();
            this.speed = speed;
            this.lifetime = lifetime;

            var ent = handle.CreateEntityNode(node.CreateChild());
            ent.Entity.Mesh = TW.Assets.LoadMesh("Scattered\\Models\\Bullet");
            ent.Node.Relative = Matrix.Scaling(2, 2, 2);

            handle.AddBehaviour(node, Update);

        }

        public void Update()
        {
            if (handle == null) return;
            Position += direction * speed * TW.Graphics.Elapsed;
            Node.Absolute = Matrix.Translation(Position);

            lifetime -= TW.Graphics.Elapsed;
            if (lifetime < 0)
            {
                Dispose();
                return;
            }

            if (Vector3.Distance(handle.LocalPlayer.Position, Position) < 0.5f)
            {
                handle.LocalPlayer.TakeDamage(0.2f);
                Dispose();
            }

        }

        public void Dispose()
        {
            if (handle == null) return;
            handle.DestroyNode(Node);
            handle = null;
        }
    }
}
