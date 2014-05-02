using DirectX11;
using MHGameWork.TheWizards.Scattered.Bindings;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using MHGameWork.TheWizards.Scattered._Engine;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.Scattered.GameLogic.Objects
{
    public class FlightEngine : IIslandAddon
    {
        private readonly Level level;
        private readonly ItemType coalType;
        public SceneGraphNode Node { get; private set; }
        public bool HasFuel { get { return fuelLeft > 0; } }

        private float fuelLeft;

        public FlightEngine(Level level, SceneGraphNode node, ItemType coalType)
        {
            this.level = level;
            this.coalType = coalType;
            Node = node;
            node.AssociatedObject = this;


            var renderNode = node.CreateChild();

            level.CreateEntityNode(renderNode.CreateChild())
                 .Alter(e => e.Node.Relative = Matrix.RotationY(-MathHelper.PiOver2) * Matrix.Translation(new Vector3(1.5f, 0, 0)))
                .Alter(ent => ent.Mesh = TW.Assets.LoadMesh("Scattered\\Models\\PropellorEngine"))
                .Alter(e => e.CreateInteractable(onInteract));

            level.CreateEntityNode(renderNode.CreateChild())
                .Alter(e => e.Node.Relative = Matrix.RotationY(-MathHelper.PiOver2) * Matrix.Translation(new Vector3(-1.5f, 0, 0)))
                .Alter(ent => ent.Mesh = TW.Assets.LoadMesh("Scattered\\Models\\PropellorEngine"))
                .Alter(e => e.CreateInteractable(onInteract));

            level.AddBehaviour(node, update);

        }

        private void onInteract()
        {
            // Toggle flight mode
            level.LocalPlayer.FlyingIsland = (Island)Node.Parent.AssociatedObject; // TODO: improve this?
            level.LocalPlayer.FlyingEngine = this;
        }

        public void PrepareForRendering()
        {
        }

        private void update()
        {
            if (level.LocalPlayer.FlyingEngine == this)
                processFuel();
        }

        private void processFuel()
        {
            fuelLeft -= TW.Graphics.Elapsed;

            if (fuelLeft < 0)
            {
                fuelLeft = 0;
                var island = (Island)Node.Parent.AssociatedObject;
                var coal = island.Addons.OfType<Resource>().FirstOrDefault(r => r.Type == coalType);
                if (coal == null) return;

                coal.Amount--;
                fuelLeft += 10;
                if (coal.Amount == 0)
                    level.DestroyNode(coal.Node);

            }
        }
    }
}