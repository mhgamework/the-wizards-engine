using System;
using System.Diagnostics;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Entity.Client;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;

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

            Vector3 transl, scaling;
            Quaternion rot;
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
            if (actor != null) return;
            actor = builder.CreateActorStatic(scene, Mesh.GetCollisionData(), worldMatrix);
            actor.GlobalPose = worldMatrix;
        }

        public void DisablePhysics()
        {
            enabledReferenceCount--;
            if (enabledReferenceCount > 0) return;
            if (enabledReferenceCount < 0) throw new InvalidOperationException("Reference count below zero!!");
            if (actor == null) return;
            actor.Dispose();
            actor = null;
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
            /*set // THIS IS A STATIC OBJECT: NO SETTER
            {
                if (actor != null)
                    throw new InvalidOperationException(
                        "This is a Static Physics object and its location can't be changed after the actor has been made!");
                worldMatrix = value;
                updateBoundingSphere();
            }*/
        }


        public Microsoft.Xna.Framework.ContainmentType ContainedInNode(ClientPhysicsQuadTreeNode _node)
        {
            return _node.NodeData.BoundingBox.Contains(boundingSphere);

        }

        #endregion
    }
}
