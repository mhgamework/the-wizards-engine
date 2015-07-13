using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Client
{
    /// <summary>
    /// 
    /// </summary>
    public class ClientPhysicsTestSphere : IClientPhysicsObject
    {

        public Vector3 Center;
        public float Radius;

        private Actor actor;

        public Actor Actor
        {
            [DebuggerStepThrough]
            get { return actor; }
        }

        private bool sleeping;

        private StillDesign.PhysX.Scene scene;

        /// <summary>
        /// This constructor disables physics
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public ClientPhysicsTestSphere( Vector3 center, float radius )
        {
            Center = center;
            Radius = radius;
        }

        /// <summary>
        /// This constructor enables physics
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public ClientPhysicsTestSphere(StillDesign.PhysX.Scene scene, Vector3 center, float radius)
        {
            this.scene = scene;
            Center = center;
            Radius = radius;
        }

        public void Move( ClientPhysicsQuadTreeNode root, Vector3 newCenter )
        {
            // NOTE: IMPORTANT: this is actually a partial implementation of the algorithm itself

            Vector3 oldCenter = Center;
            ClientPhysicsQuadTreeNode oldNode = Node;


            // Update location in quadtree
            Center = newCenter;
            root.OrdenObject( this );

            // Update dynamic object count

            Node.AddDynamicObjectToIntersectingNodes( this ); // Add must come before remove to prevent overhead

            if ( oldNode != null )
            {
                Center = oldCenter; // set old state
                oldNode.RemoveDynamicObjectFromIntersectingNodes( this );

                Center = newCenter; // set new state
            }
        }

        public void Update( ClientPhysicsQuadTreeNode root, IXNAGame game )
        {
            if ( sleeping && actor.IsSleeping ) return;
            if ( sleeping && !actor.IsSleeping )
            {
                // Make dynamic again!
                sleeping = false;
                Node.AddDynamicObjectToIntersectingNodes( this );
            }

            if ( actor.IsSleeping && !sleeping )
            {
                // Disable dynamic, thus make static
                if ( node != null )
                    node.RemoveDynamicObjectFromIntersectingNodes( this );

                sleeping = true;
                if ( node != null && node.PhysicsEnabled == false )
                    DisablePhysics(); // disable movement
                return;
            }


            Move( root, actor.GlobalPosition );

        }

        public void InitDynamic()
        {
            if ( actor != null ) return;
            SphereShapeDescription shapeDescription = new SphereShapeDescription( Radius );
            ActorDescription actorDescription = new ActorDescription( shapeDescription );

            actorDescription.BodyDescription = new BodyDescription( 2f );

            actorDescription.GlobalPose = Matrix.CreateTranslation( Center );

            actor = scene.CreateActor( actorDescription );
        }


        #region IClientPhysicsObject Members

        /// <summary>
        /// Note that this is wierd in the sense that the clientPhysics will not call this method because this is an dynamic object
        /// </summary>
        public void EnablePhysics()
        {
            if ( !sleeping ) return;
            actor.BodyFlags.Kinematic = false;
        }

        public void DisablePhysics()
        {

            if ( !sleeping ) return;
            actor.BodyFlags.Kinematic = true;
        }




        private ClientPhysicsQuadTreeNode node;
        public ClientPhysicsQuadTreeNode Node
        {
            get { return node; }
            set { node = value; }
        }

        public ContainmentType ContainedInNode( ClientPhysicsQuadTreeNode _node )
        {
            return _node.NodeData.BoundingBox.xna()   .Contains( new BoundingSphere( Center, Radius ) );
        }

        #endregion
    }
}
