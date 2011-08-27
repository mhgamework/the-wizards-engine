using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Client;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Player.Client
{
    /// <summary>
    /// This class implements the IClientPhysicsObject interface, thus represents the client physics object for a player.
    /// </summary>
    public class PlayerClientPhysics : IClientPhysicsObject
    {
        private Vector3 currentPosition;
        private PlayerController controller;

        public PlayerClientPhysics( PlayerController _controller )
        {
            controller = _controller;
            currentPosition = controller.RetrievePosition();
        }


        /// <summary>
        /// This updates the position of the client physics object to the controller's position, 
        /// and updates the dynamicobjectscount in the Client Physics
        /// This uses the dynamic object trick
        /// </summary>
        public void Update( ClientPhysicsQuadTreeNode root )
        {
            // First adds count, then removes. This is simply to ensure that objects are not flicked on of this frame

            Vector3 oldPosition = currentPosition;
            Vector3 newPosition = controller.RetrievePosition();
            currentPosition = newPosition;

            ClientPhysicsQuadTreeNode oldNode = node;

            root.OrdenObject( this );

            node.AddDynamicObjectToIntersectingNodes( this );

            if ( oldNode != null )
            {
                currentPosition = oldPosition;
                oldNode.RemoveDynamicObjectFromIntersectingNodes( this );
                currentPosition = newPosition;
            }
            


        }

        #region IClientPhysicsObject Members

        public void EnablePhysics()
        {
            // Always enabled for this dynamic object
        }

        public void DisablePhysics()
        {
            // always enabled
        }

        private ClientPhysicsQuadTreeNode node;
        public ClientPhysicsQuadTreeNode Node
        {
            get
            {
                return node;
            }
            set
            {
                node = value;
            }
        }

        public Microsoft.Xna.Framework.ContainmentType ContainedInNode( ClientPhysicsQuadTreeNode _node )
        {
            //TODO: make this the actual size of the controller
            BoundingBox bb = new BoundingBox( currentPosition + new Vector3( -0.5f, -0.5f, -0.5f ), currentPosition + new Vector3( 0.5f, 0.5f, 0.5f ) );

            return _node.NodeData.BoundingBox.xna().Contains(bb);

        }

        #endregion
    }
}
