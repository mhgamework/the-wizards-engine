using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards._XNA.Gameplay;
using MHGameWork.TheWizards.Client;
using SlimDX;
using StillDesign.PhysX;
using ContainmentType = Microsoft.Xna.Framework.ContainmentType;

namespace MHGameWork.TheWizards.Player
{
    /// <summary>
    /// Responsible for incorporating a playercontroller into the clientphysics
    /// </summary>
    class PlayerClientPhysics : IClientPhysicsObject
    {
        private readonly PlayerController controller;

        /// <summary>
        /// This constructor enables physics
        /// </summary>
        public PlayerClientPhysics(PlayerController controller)
        {
            this.controller = controller;
        }

        /// <summary>
        /// This is the position of the ClientPhyiscs actor, not the playercontroller (so might be One frame behind)
        /// </summary>
        private Matrix world;

        /// <summary>
        /// TODO: combine all instances of this into one method
        /// </summary>
        /// <param name="root"></param>
        /// <param name="newPose"></param>
        private void move(ClientPhysicsQuadTreeNode root, Matrix newPose)
        {

            // NOTE: IMPORTANT: this is actually a partial implementation of the algorithm itself

            Matrix oldPose = world;
            ClientPhysicsQuadTreeNode oldNode = Node;


            // Update location in quadtree
            world = newPose;
            root.OrdenObject(this);

            // Update dynamic object count

            Node.AddDynamicObjectToIntersectingNodes(this); // Add must come before remove to prevent overhead

            if (oldNode != null)
            {
                world = oldPose; // set old state
                oldNode.RemoveDynamicObjectFromIntersectingNodes(this);

                world = newPose;  // set new state
            }
        }

        public void Update(ClientPhysicsQuadTreeNode root)
        {

            move(root, Matrix.Translation(controller.GlobalPosition.dx()));



        }

        private float boundingRadius = 2;
        public BoundingSphere GetBoundingSphere()
        {
            return new BoundingSphere(world.xna().Translation.dx(), boundingRadius);
        }

        #region IClientPhysicsObject Members

        public void EnablePhysics()
        {
        }

        public void DisablePhysics()
        {
        }




        private ClientPhysicsQuadTreeNode node;
        public ClientPhysicsQuadTreeNode Node
        {
            get { return node; }
            set { node = value; }
        }

        public ContainmentType ContainedInNode(ClientPhysicsQuadTreeNode _node)
        {
            return _node.NodeData.BoundingBox.xna().Contains(GetBoundingSphere().xna());
        }

        #endregion

        internal void disposeInternal()
        {
            //TODO:
            node.RemoveDynamicObjectFromIntersectingNodes(this);
        }


    }
}
