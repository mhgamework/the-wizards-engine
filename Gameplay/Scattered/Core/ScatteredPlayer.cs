using System;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using SlimDX;
using MHGameWork.TheWizards.Scattered._Engine;
using MHGameWork.TheWizards.SkyMerchant._Engine;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class ScatteredPlayer
    {
        private readonly SceneGraphNode node;
        private Vector3 position;
        private Vector3 direction;
        private EntityNode itemNode;
        private ItemType heldItem;

        public ScatteredPlayer(Level level, SceneGraphNode node)
        {
            this.node = node;

            itemNode = level.CreateEntityNode(node.CreateChild().Alter(c => c.Relative = Matrix.Translation(0, -0.5f, -3)));
        }

        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                updateNode();
            }
        }

        private void updateNode()
        {
            node.Absolute = Matrix.Invert(Matrix.LookAtRH(position, position + direction, Vector3.UnitY));
        }

        public Vector3 Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                updateNode();
            }
        }

        /// <summary>
        /// Contains the island the player is currently flying
        /// </summary>
        public Island FlyingIsland { get; set; }

        public ItemType HeldItem
        {
            get { return heldItem; }
            set
            {
                heldItem = value;
                itemNode.Entity.Mesh = heldItem.With(i => i.Mesh);
            }
        }
    }
}