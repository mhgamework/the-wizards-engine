using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    public class WereldMeshPool 
    {
      

        private IDList<DatabaseMesh> meshes;

        public IDList<DatabaseMesh> Meshes
        {
            get { return meshes; }
            set { meshes = value; }
        }

        public WereldMeshPool()
        {
            meshes = new IDList<DatabaseMesh>();
        }


        public void SaveToXml( TWXmlNode node )
        {
            TWXmlNode meshesNode = node.CreateChildNode( "Meshes" );
            meshesNode.AddAttributeInt( "count", meshes.Count );
            for ( int i = 0; i < meshes.Count; i++ )
            {
                meshes.GetByIndex( i ).SaveToXml( meshesNode.CreateChildNode( "Mesh" ) );
            }
        }

        public void LoadFromXml( TWXmlNode node )
        {
        }

    }
}
