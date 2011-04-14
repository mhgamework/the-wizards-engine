using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient
{
    public class DatabaseModel : IXmlSerializable, IUnique
    {
        private int id;
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private Matrix worldMatrix;

        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set { worldMatrix = value; }
        }

        private BoundingSphere boundingSphere;
        public BoundingSphere BoundingSphere
        {
            get { return BoundingSphere; }
            set { BoundingSphere = value; }
        }

        public DatabaseModel()
        {
            id = -1;
            worldMatrix = Matrix.Identity;
            boundingSphere = new BoundingSphere( Vector3.Zero, 1 );

        }


        #region IXmlSerializable Members

        public void SaveToXml( TWXmlNode node )
        {
            node.AddChildNode( "ID", id.ToString() );
            XMLSerializer.WriteMatrix( node.CreateChildNode( "WorldMatrix" ), worldMatrix );
            XMLSerializer.WriteBoundingSphere( node.CreateChildNode( "BoundingSphere" ), boundingSphere );

        }

        #endregion

  
    }
}
