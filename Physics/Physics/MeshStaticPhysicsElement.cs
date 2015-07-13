using System;
using System.Diagnostics;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Physics
{
    /// <summary>
    /// This class implements the IClientPhysics interface, thus representing a client physics mesh. 
    /// These objects are static by default. 
    /// </summary>
    public class MeshStaticPhysicsElement : IClientPhysicsObject
    {
        public IMesh Mesh { get; set; }
        private MeshPhysicsActorBuilder builder;
        private StillDesign.PhysX.Scene scene;
        private StillDesign.PhysX.Actor actor;
        private BoundingSphere boundingSphere;
        public BoundingSphere BoundingSphere
        {
            get { return boundingSphere; }
        }

        private object actorUserData;
        public object ActorUserData
        {
            get { return actorUserData; }
            set { actorUserData = value; if (Actor != null) Actor.UserData = actorUserData; }
        }


        /// <summary>
        /// World is in the constructor because this is a static physx object
        /// </summary>
        /// <param name="_mesh"></param>
        /// <param name="world"></param>
        public MeshStaticPhysicsElement(IMesh _mesh, Matrix world, MeshPhysicsActorBuilder _builder)
        {
            Mesh = _mesh;
            builder = _builder;


            worldMatrix = world;

            updateBoundingSphere();

        }

        private void updateBoundingSphere()
        {
            boundingSphere = builder.CalculateBoundingSphere(Mesh.GetCollisionData());

            boundingSphere.Center = Vector3.Transform(boundingSphere.Center, worldMatrix);
            //SCALING NOT SUPPORTED

            //Vector3 transl, scaling;
            //Quaternion rot;
            /*model.ObjectMatrix.Decompose(out scaling, out rot, out transl);
            bs.Radius *= MathHelper.Max(MathHelper.Max(scaling.X, scaling.Y), scaling.Z);
            scaling = entityFullData.Transform.Scaling;
            bs.Radius *= MathHelper.Max(MathHelper.Max(scaling.X, scaling.Y), scaling.Z);

            if (i == 0)
                boundingSphere = bs;
            else
                boundingSphere = BoundingSphere.CreateMerged(boundingSphere, bs);*/
        }

        public void LoadInClientPhysics(StillDesign.PhysX.Scene _scene, ClientPhysicsQuadTreeNode root)
        {
            scene = _scene;
            //root.OrdenObject(this);
            root.AddStaticObject(this);
        }

        #region IClientPhysicsObject Members

        private int enabledReferenceCount = 0;

        public void EnablePhysics()
        {
            enabledReferenceCount++;
            if (Actor != null) return;
            Actor = builder.CreateActorStatic(scene, Mesh.GetCollisionData(), worldMatrix);
            Actor.GlobalPose = worldMatrix;
            Actor.UserData = actorUserData;
            if (ActorCreated != null) ActorCreated(Actor);
        }

        public void DisablePhysics()
        {
            enabledReferenceCount--;
            if (enabledReferenceCount > 0) return;
            if (enabledReferenceCount < 0) throw new InvalidOperationException("Reference count below zero!!");
            if (Actor == null) return;
            Actor.Dispose();
            Actor = null;
        }

        private ClientPhysicsQuadTreeNode node;

        public ClientPhysicsQuadTreeNode Node
        {
            get { return node; }
            set { node = value; }
        }

        private Matrix worldMatrix;

        /// <summary>
        /// WARNING: SCALING NOT SUPPORTED
        /// </summary>
        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set
            {
                if (node == null)
                {
                    worldMatrix = value;
                    return;
                }
                var oldNode = node;

                node.RemoveStaticObject(this);
                worldMatrix = value;
                updateBoundingSphere();
                var newNode = oldNode.FindContainingNodeUpwards(this);
                if (newNode == null)
                    newNode = QuadTree.GetRootNode(oldNode);

             
                newNode.AddStaticObject(this);
                if (Actor != null)
                    Actor.GlobalPose = worldMatrix;
            }
        }

        public Actor Actor
        {
            get { return actor; }
            private set { actor = value; }
        }


        public Microsoft.Xna.Framework.ContainmentType ContainedInNode(ClientPhysicsQuadTreeNode _node)
        {
            return _node.NodeData.BoundingBox.xna().Contains(boundingSphere);

        }

        #endregion


        internal void disposeInternal()
        {
            if (node != null)
                node.RemoveStaticObject(this);
            builder = null;
            scene = null;
            if (Actor != null)
                Actor.Dispose();
            Actor = null;
        }


        public event Action<Actor> ActorCreated;

    }
}
