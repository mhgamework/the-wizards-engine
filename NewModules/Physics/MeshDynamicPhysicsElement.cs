using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Entity.Client;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Physics
{
    public class MeshDynamicPhysicsElement : IClientPhysicsObject
    {
        public IMesh Mesh { get; private set; }
        public Matrix World { get; private set; }
        public MeshPhysicsActorBuilder Builder { get; private set; }


        private Actor actor;

        public Actor Actor
        {
            [DebuggerStepThrough]
            get { return actor; }
        }

        private bool sleeping;

        private StillDesign.PhysX.Scene scene;

   
        /// <summary>
        /// This constructor enables physics
        /// </summary>
        public MeshDynamicPhysicsElement( IMesh mesh, Matrix world, MeshPhysicsActorBuilder builder)
        {
            Mesh = mesh;
            World = world;
            Builder = builder;
        }

        public void Move(ClientPhysicsQuadTreeNode root, Matrix newPose)
        {
            if (actor == null) throw new InvalidOperationException();
            // NOTE: IMPORTANT: this is actually a partial implementation of the algorithm itself

            Matrix oldPose = World;
            ClientPhysicsQuadTreeNode oldNode = Node;


            // Update location in quadtree
            World  = newPose;
            root.OrdenObject(this);

            // Update dynamic object count

            Node.AddDynamicObjectToIntersectingNodes(this); // Add must come before remove to prevent overhead

            if (oldNode != null)
            {
                World = oldPose;; // set old state
                oldNode.RemoveDynamicObjectFromIntersectingNodes(this);

                World = newPose;; // set new state
            }
        }

        public void Update(ClientPhysicsQuadTreeNode root, IXNAGame game)
        {
            if (sleeping && actor.IsSleeping) return;
            if (sleeping && !actor.IsSleeping)
            {
                // Make dynamic again!
                sleeping = false;
                Node.AddDynamicObjectToIntersectingNodes(this);
            }

            if (actor.IsSleeping && !sleeping)
            {
                // Disable dynamic, thus make static
                if (node != null)
                    node.RemoveDynamicObjectFromIntersectingNodes(this);

                sleeping = true;
                if (node != null && node.PhysicsEnabled == false)
                    DisablePhysics(); // disable movement
                return;
            }


            Move(root, actor.GlobalPose);

        }

        public void InitDynamic(StillDesign.PhysX.Scene _scene)
        {
            if (actor != null) throw new InvalidOperationException();

            scene = _scene;
            actor = Builder.CreateActorDynamic(scene, Mesh.GetCollisionData(), World);
            var bs = Microsoft.Xna.Framework.BoundingSphere.CreateFromBoundingBox(Builder.CalculateBoundingBox(Mesh.GetCollisionData()));
            boundingRadius = bs.Radius;
        }

        private float boundingRadius;
        public BoundingSphere GetBoundingSphere()
        {
            return new BoundingSphere(World.Translation, boundingRadius);
        }

        #region IClientPhysicsObject Members

        /// <summary>
        /// Note that this is wierd in the sense that the clientPhysics will not call this method because this is an dynamic object
        /// </summary>
        public void EnablePhysics()
        {
            if (!sleeping) return;
            actor.BodyFlags.Kinematic = false;
        }

        public void DisablePhysics()
        {

            if (!sleeping) return;
            actor.BodyFlags.Kinematic = true;
        }




        private ClientPhysicsQuadTreeNode node;
        public ClientPhysicsQuadTreeNode Node
        {
            get { return node; }
            set { node = value; }
        }

        public ContainmentType ContainedInNode(ClientPhysicsQuadTreeNode _node)
        {
            return _node.NodeData.BoundingBox.Contains(GetBoundingSphere());
        }

        #endregion

        internal void disposeInternal()
        {
            if (actor != null)
            {
                actor.Dispose();
                node.RemoveDynamicObjectFromIntersectingNodes(this);
                
            }
        }
    }
}
