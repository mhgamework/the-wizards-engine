using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    public class DatabaseOud : IXmlSerializable
    {
        private IDList<DatabaseEntity> entities;
        private IDList<DatabaseMesh> meshes;

        public DatabaseOud()
        {
            entities = new IDList<DatabaseEntity>();
            meshes = new IDList<DatabaseMesh>();
        }

        public void SaveToXml( TWXmlNode node )
        {
            TWXmlNode entitiesNode = node.CreateChildNode( "Entities" );
            entitiesNode.AddAttribute( "count", entities.Count.ToString() );
            for ( int i = 0; i < entities.Count; i++ )
            {
                DatabaseEntity entity = entities.GetByIndex( i );
                TWXmlNode entityNode = entitiesNode.CreateChildNode( "Entity" );
                entity.SaveToXml( entityNode );
            }


            TWXmlNode meshesNode = node.CreateChildNode( "Meshes" );
            meshesNode.AddAttribute( "count", meshes.Count.ToString() );
            for ( int i = 0; i < meshes.Count; i++ )
            {
                DatabaseMesh mesh = meshes.GetByIndex( i );
                TWXmlNode meshNode = meshesNode.CreateChildNode( "Mesh" );
                mesh.SaveToXml( meshNode );
            }

        }

        public void LoadFromXml( TWXmlNode node )
        {
            TWXmlNode entitiesNode = node.FindChildNode( "Entities" );

            // count not used yet.
            int count = entitiesNode.GetAttributeInt( "count" );

            TWXmlNode[] entityNodes = entitiesNode.FindChildNodes( "Entity" );

            for ( int i = 0; i < count; i++ )
            {
                DatabaseEntity entity = new DatabaseEntity();
                entity.LoadFromXml( entityNodes[ i ] );
                entities.Add( entity );


            }

        }

























        public static void TestSerialize()
        {
            DatabaseOud db = new DatabaseOud();

            DatabaseEntity entity = new DatabaseEntity();
            entity.GameFileID = 5;
            db.entities.AddNew( entity );


            //DatabaseMesh mesh = new DatabaseMesh();
            //mesh.VertexStride = 99;

            //db.meshes.AddNew( mesh );

            //mesh = new DatabaseMesh();
            //mesh.VertexStride = 2;

            //db.meshes.AddNew( mesh );




            System.Xml.XmlDocument doc = TWXmlNode.CreateXmlDocument();
            TWXmlNode databaseNode = new TWXmlNode( doc, "Database" );

            db.SaveToXml( databaseNode );

            System.IO.FileStream fs = null;
            try
            {
                fs = new System.IO.FileStream( System.Windows.Forms.Application.StartupPath + @"\UnitTests\Database_TestSerialize.xml"
                   , System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Delete );

                doc.Save( fs );
            }
            finally
            {
                if ( fs != null )
                    fs.Close();
            }


        }

        public static void TestWereld001()
        {
            //DatabaseOud wereld = null;

            //TestXNAGame.Start( "TestWereld001",
            //    delegate
            //    {
            //        wereld = new DatabaseOud();

            //        Entity ent = wereld.CreateNewEntity();

            //        DatabaseModel model = ent.CreateNewModel();



            //    },
            //    delegate
            //    {
            //        wereld.Update();
            //        wereld.Render();
            //    }
            //);
        }
    }
}
