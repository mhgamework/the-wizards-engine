using DirectX11;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using MHGameWork.TheWizards.Scattered._Engine;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class FlightEngine : IIslandAddon
    {
        public SceneGraphNode Node { get; private set; }

        public FlightEngine(Level level, SceneGraphNode node)
        {
            Node = node;


            var renderNode = node.CreateChild();

            level.CreateEntityNode(renderNode.CreateChild())
                 .Alter(e => e.Node.Relative = Matrix.RotationY(-MathHelper.PiOver2) * Matrix.Translation(new Vector3(1.5f, 0, 0)))
                .Alter(ent => ent.Entity.Mesh = TW.Assets.LoadMesh("Scattered\\Models\\PropellorEngine"))
                .Alter(e => e.CreateInteractable(onInteract));

            level.CreateEntityNode(renderNode.CreateChild())
                .Alter(e => e.Node.Relative = Matrix.RotationY(-MathHelper.PiOver2) * Matrix.Translation(new Vector3(-1.5f, 0, 0)))
                .Alter(ent => ent.Entity.Mesh = TW.Assets.LoadMesh("Scattered\\Models\\PropellorEngine"))
                .Alter(e => e.CreateInteractable(onInteract));

        }

        private void onInteract()
        {
            // Toggle flight mode
            Node.Relative *= Matrix.Translation(1, 0, 0);
        }
    }
}