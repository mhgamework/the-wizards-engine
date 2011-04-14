using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.Wereld
{
    public class ClientEntityHolder
    {
        private IClientEntity entity;
        private QuadTreeNode node;
        private List<IHolderElement> elements;
        private int id = -1;
        private IBody body;

        public ClientEntityHolder( IClientEntity nEntity )
        {
            entity = nEntity;
            node = null;

            elements = new List<IHolderElement>();
        }

        public void SetID( int nID )
        {
            id = nID;
        }

        public void MoveToNode( QuadTreeNode nNode )
        {
            //if ( node == nNode ) return;     //Deze regel kan veroorzaken dat entities verdwijnen uit de quadtree


            if ( node != null )
            {
                node.RemoveEntity( this );
                //the node's bounding y zou moeten worden geupdate van tijd tot tijd
            }

            if ( nNode != null )
            {
                nNode.AddEntity( this );



            }
            node = nNode;
        }

        public void AddElement( IHolderElement nIHE )
        {
            elements.Add( nIHE );
            nIHE.SetEntityHolder( this );

            if ( nIHE is IBody ) body = (IBody)nIHE;

        }

        public void Render()
        {
            entity.Render();
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



        //Client Specific
        public void UpdateEntity( int nTick, Common.Network.UpdateEntityPacket p )
        {
            if ( body != null ) body.AddEntityUpdate( nTick, p );


            //body.Positie = p.Positie;
            //body.RotatieQuat = p.RotatieQuat;
        }

        public void UpdateEntity( int nTick, Common.ByteReader br, int length )
        {
            UpdateEntity( nTick, Common.Network.UpdateEntityPacket.FromNetworkBytes( br ) );
        }



        public BoundingSphere BoundingSphere
        { get { return entity.BoundingSphere; } }

        public BoundingBox BoundingBox
        { get { return Microsoft.Xna.Framework.BoundingBox.CreateFromSphere( entity.BoundingSphere ); } }

        public QuadTreeNode Node { get { return node; } }

        public IBody Body { get { return body; } }

        public int ID { get { return id; } }

        public IClientEntity Entity { get { return entity; } }
    }
}
