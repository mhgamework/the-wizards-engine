using System;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Lod;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Simulation.Spatial;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.SceneGraphing
{
    /// <summary>
    /// This is the Lod implementation of an entity node
    /// </summary>
    public class EntityNode : IRenderable
    {
        private readonly Level level;
        private readonly OptimizedWorldOctree<IRenderable> tree;
        public SceneGraphNode Node { get; private set; }
        private Entity Entity;

        public EntityNode(Level level, SceneGraphNode node, OptimizedWorldOctree<IRenderable> tree)
        {
            this.level = level;
            this.tree = tree;
            Node = node;
            visible = false;// Default to invisible
            node.ObserveChange(onNodeChange);
        }

        private void createEntity()
        {
            if (Entity == null)
                Entity = new Entity();
            updateEntity();
        }

        private void updateEntity()
        {
            if (Entity == null) return;
            Entity.Visible = visible;
            Entity.Mesh = mesh;
            Entity.WorldMatrix = Node.Absolute;
        }

        private void onNodeChange()
        {
            updateEntity();
            tree.UpdateWorldObject(this);
        }

        /// <summary>
        /// Creates an interactable which allows interacting with this entity.
        /// Currently this creates a boundingboxinteractable around the mesh of this entity.
        /// </summary>
        /// <param name="onInteract"></param>
        public void CreateInteractable(Action onInteract)
        {
            //throw new NotImplementedException(); // Do not pass entities around anymore
            level.CreateEntityInteractable(this, Node.CreateChild(), onInteract);
        }

        public void Dispose()
        {
            if (Entity != null)
                TW.Data.RemoveObject(Entity);
            Entity = null;
        }

        public BoundingBox BoundingBox
        {
            get
            {
                if (Mesh == null) return new BoundingBox();
                return TW.Assets.GetBoundingBox(Mesh).Transform(Node.Absolute);
            }
        }

        private bool visible;
        public bool Visible
        {
            get { return visible; }
            set
            {
                visible = value;
                if (visible) createEntity();
                // TODO: add some way to remove an entity when far enough
            }
        }

        private IMesh mesh;
        public IMesh Mesh
        {
            get { return mesh; }
            set
            {
                var change = mesh != value;
                mesh = value;
                updateEntity();
                if (change) onNodeChange();
            }
        }
    }
}