using System;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.Scattered.SceneGraphing
{
    public class EntityNode
    {
        private readonly Level level;
        public SceneGraphNode Node { get; private set; }
        public Entity Entity { get; private set; }

        public EntityNode(Level level,SceneGraphNode node)
        {
            this.level = level;
            Node = node;
            Entity = new Entity();
        }

        public void UpdateForRendering()
        {
            Entity.WorldMatrix = Node.Absolute;
        }

        /// <summary>
        /// Creates an interactable which allows interacting with this entity.
        /// Currently this creates a boundingboxinteractable around the mesh of this entity.
        /// </summary>
        /// <param name="onInteract"></param>
        public void CreateInteractable(Action onInteract)
        {
            var ret = level.CreateEntityInteractable(Entity, Node.CreateChild(),onInteract);
        }
    }
}