using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Server.Wereld
{
    [Obsolete]
    public class ServerEntityHolder : IEntityHolder
    {
        private Wereld wereld;
        private IServerEntity entity;
        private QuadTreeNode containingNode;
        private List<QuadTreeNode> collisionNodes;
        private List<IHolderElement> elements;
        private int id = -1;
        private bool _static = false;

        private IBody body;

        public ServerEntityHolder( IServerEntity nEntity )
        {
            entity = nEntity;
            containingNode = null;
            elements = new List<IHolderElement>();
            collisionNodes = new List<QuadTreeNode>( 4 );
        }

        public void SetID( int nID )
        {
            id = nID;
        }

        public void SetWereld( Wereld nWereld )
        {
            wereld = nWereld;
        }

        public Common.Networking.INetworkSerializable GetEntityUpdatePacket()
        {
            MHGameWork.TheWizards.Common.Network.UpdateEntityPacket p = new MHGameWork.TheWizards.Common.Network.UpdateEntityPacket();

            //p.EntityID = id;
            p.Positie = body.Positie;
            p.RotatieQuat = body.RotatieQuat;

            return p;
        }


        public void MoveToNode( QuadTreeNode nNode )
        {
            //if ( node == nNode ) return;     //Deze regel kan veroorzaken dat entities verdwijnen uit de quadtree


            if ( containingNode != null )
            {
                containingNode.RemoveEntity( this );

            }

            if ( nNode != null )
            {
                nNode.AddEntity( this );



            }
            containingNode = nNode;


            //Update collision nodes


            QuadTreeNode[] oldCollisionNodes = collisionNodes.ToArray();
            collisionNodes.Clear();

            wereld.Tree.FindCollisionNodes( containingNode, this, collisionNodes );

            //First add the new references, them remove the old ones.
            for ( int i = 0; i < collisionNodes.Count; i++ )
            {
                collisionNodes[ i ].AddDynamicEntityReference();
            }

            for ( int i = 0; i < oldCollisionNodes.Length; i++ )
            {
                oldCollisionNodes[ i ].RemoveDynamicEntityReference();
            }


        }

        public void AddElement( IHolderElement nIHE )
        {
            elements.Add( nIHE );
            nIHE.SetEntityHolder( this );

            if ( nIHE is IBody ) body = (IBody)nIHE;
        }

        public void Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            entity.Process( e );
            for ( int i = 0; i < elements.Count; i++ )
            {
                elements[ i ].Process( e );
            }
        }
        public void Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        {
            entity.Tick( e );
            for ( int i = 0; i < elements.Count; i++ )
            {
                elements[ i ].Tick( e );
            }
        }


        /// <summary>
        /// This is called by the Body instance of this entity Holder.
        /// It occurs when the position, orientation, scaling,... the physical property of this entity change
        /// </summary>
        public void OnBodyChanged()
        {
            if ( containingNode != null )
            {
                ServerMainNew.Instance.Wereld.Tree.OrdenEntity( this );
            }
        }


        public void EnablePhysics()
        {
            entity.EnablePhysics();
        }
        public void DisablePhysics()
        {
            entity.DisablePhysics();
        }



        public BoundingSphere BoundingSphere
        { get { return entity.BoundingSphere; } }

        public BoundingBox BoundingBox
        { get { return Microsoft.Xna.Framework.BoundingBox.CreateFromSphere( entity.BoundingSphere ); } }

        public QuadTreeNode ContainingNode
        { get { return containingNode; } }
        public List<QuadTreeNode> CollisionNodes
        { get { return collisionNodes; } }

        public IBody Body
        { get { return body; } }

        public int ID { get { return id; } }

        public bool Static
        {
            get { return _static; }
            set
            {
                _static = value;
            }
        }

        public Wereld Wereld
        {
            get { return wereld; }
        }
    }
}
