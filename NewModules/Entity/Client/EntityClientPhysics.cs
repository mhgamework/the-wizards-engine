using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Entities;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Entity.Client
{
    /// <summary>
    /// This class implements the IClientPhysics interface, thus representing a client physics entity. 
    /// These objects are static by default. Support for dynamic entities should be implemented at a later time.
    /// </summary>
    [Obsolete("This is now moved to the Meshes")]
    public class EntityClientPhysics : IClientPhysicsObject
    {
        private EntityFullData entityFullData;
        private EntityPhysicsActorBuilder builder;
        private StillDesign.PhysX.Scene scene;
        private StillDesign.PhysX.Actor actor;
        private BoundingSphere boundingSphere;

        public BoundingSphere BoundingSphere
        {
            get { return boundingSphere; }
        }

        public EntityClientPhysics( EntityFullData entityFullData )
        {
            this.entityFullData = entityFullData;
            builder = new EntityPhysicsActorBuilder();

            for (int i = 0; i < entityFullData.ObjectFullData.Models.Count ; i++)
            {
                ModelFullData model = entityFullData.ObjectFullData.Models[ i ];

                // Transformation scalling is somewhat decomposed here. See EntityPhysicsActorBuilder for more details
                BoundingSphere bs;

                bs = model.BoundingSphere;
                bs.Center = Vector3.Transform( bs.Center, model.ObjectMatrix * entityFullData.Transform.CreateMatrix() );
                Vector3 transl, scaling;
                Quaternion rot;
                model.ObjectMatrix.Decompose( out scaling, out rot, out transl );
                bs.Radius *= MathHelper.Max( MathHelper.Max( scaling.X, scaling.Y ), scaling.Z );
                scaling = entityFullData.Transform.Scaling;
                bs.Radius *= MathHelper.Max( MathHelper.Max( scaling.X, scaling.Y ), scaling.Z );

                if ( i == 0 )
                    boundingSphere = bs;
                else
                    boundingSphere = BoundingSphere.CreateMerged(boundingSphere, bs);
            }


            
        }

        public void LoadInClientPhysics( StillDesign.PhysX.Scene _scene, ClientPhysicsQuadTreeNode root )
        {
            scene = _scene;
            root.OrdenObject( this );
        }

        #region IClientPhysicsObject Members

        public void EnablePhysics()
        {
            if ( actor != null ) return;
            actor = builder.CreateActorForEntity( scene, entityFullData );
        }

        public void DisablePhysics()
        {
            if ( actor == null ) return;
            actor.Dispose();
            actor = null;
        }

        private ClientPhysicsQuadTreeNode node;

        public ClientPhysicsQuadTreeNode Node
        {
            [DebuggerStepThrough]
            get { return node; }
            [DebuggerStepThrough]
            set { node = value; }
        }


        public Microsoft.Xna.Framework.ContainmentType ContainedInNode( ClientPhysicsQuadTreeNode _node )
        {
            return _node.NodeData.BoundingBox.Contains( boundingSphere );

        }

        #endregion
    }
}
