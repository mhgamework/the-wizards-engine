using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using MHGameWork.TheWizards.Scattered._Engine;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class ResourceGenerator : IIslandAddon
    {
        private readonly Level level;
        private readonly ItemType type;

        public ResourceGenerator(Level level, SceneGraphNode node, ItemType type)
        {
            this.level = level;
            this.type = type;
            Node = node;
            node.AssociatedObject = this;

            level.CreateEntityNode(node.CreateChild())
                 .Alter(k => k.Entity.Mesh = TW.Assets.LoadMesh("Scattered\\Models\\Infuser"))
                 .Alter(k => k.CreateInteractable(onInteract));

            containsResourcesSince = TW.Graphics.TotalRunTime;
        }

        private float containsResourcesSince;

        private void onInteract()
        {
            if (containsResourcesSince < 3) return;
            containsResourcesSince += 3;
            // Should be in the update method.
            if (containsResourcesSince + 9 < TW.Graphics.TotalRunTime)
                containsResourcesSince = TW.Graphics.TotalRunTime - 9;

            level.LocalPlayer.Inventory.AddNewItems(type, 1);


        }

        public SceneGraphNode Node { get; private set; }
        public void PrepareForRendering()
        {
            
        }
    }
}