using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient
{
    public class WereldMesh : IUnique
    {
        private TWWereld wereld;

        public TWWereld Wereld
        {
            get { return wereld; }
            set { wereld = value; }
        }

        private int id = -1;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private int vertexCount;

        public int VertexCount
        {
            get { return vertexCount; }
            set { vertexCount = value; }
        }
        private int triangleCount;

        public int TriangleCount
        {
            get { return triangleCount; }
            set { triangleCount = value; }
        }



        private VertexDeclaration vertexDeclaration;
        public VertexDeclaration VertexDeclaration
        {
            get { return vertexDeclaration; }
            set { vertexDeclaration = value; }
        }

        private int vertexStride;

        public int VertexStride
        {
            get { return vertexStride; }
            set { vertexStride = value; }
        }

        private VertexBuffer vertexBuffer;

        public VertexBuffer VertexBuffer
        {
            get { return vertexBuffer; }
            set { vertexBuffer = value; }
        }

        private IndexBuffer indexBuffer;

        public IndexBuffer IndexBuffer
        {
            get { return indexBuffer; }
            set { indexBuffer = value; }
        }

        public WereldMesh( TWWereld nWereld )
        {
            wereld = nWereld;
        }


        public void DrawPrimitives()
        {
            wereld.Game.GraphicsDevice.Vertices[ 0 ].SetSource( VertexBuffer, 0, VertexStride );
            wereld.Game.GraphicsDevice.Indices = IndexBuffer;
            wereld.Game.GraphicsDevice.VertexDeclaration = VertexDeclaration;
            wereld.Game.GraphicsDevice.DrawIndexedPrimitives( PrimitiveType.TriangleList, 0, 0, VertexCount, 0, TriangleCount );
        }


        public void SaveToXml( TWXmlNode node )
        {
            node.AddChildNode( "ID", id.ToString() );
            node.AddChildNode( "VertexCount", vertexCount.ToString() );
            node.AddChildNode( "TriangleCount", triangleCount.ToString() );
            XMLSerializer.WriteVertexDeclaration( node.CreateChildNode( "VertexDeclaration" ), vertexDeclaration );
            node.AddChildNode( "VertexStride", vertexStride.ToString() );
            XMLSerializer.WriteVertexBuffer( node.CreateChildNode( "VertexBuffer" ), vertexBuffer );
            XMLSerializer.WriteIndexBuffer( node.CreateChildNode( "IndexBuffer" ), indexBuffer );

        }

        public void LoadFromXml( TWXmlNode node )
        {
            id = int.Parse( node.ReadChildNodeValue( "ID" ) );
            vertexCount = int.Parse( node.ReadChildNodeValue( "VertexCount" ) );
            triangleCount = int.Parse( node.ReadChildNodeValue( "TriangleCount" ) );
            vertexDeclaration = XMLSerializer.ReadVertexDeclaration( wereld.Game, node.FindChildNode( "VertexDeclaration" ) );
            vertexStride = int.Parse( node.ReadChildNodeValue( "VertexStride" ) );
            VertexBuffer = XMLSerializer.ReadVertexBuffer( node.FindChildNode( "VertexBuffer" ), wereld.Game );
            indexBuffer = XMLSerializer.ReadIndexBuffer( node.FindChildNode( "IndexBuffer" ), wereld.Game );

        }


        public static void TestSerialize()
        {
            TestXNAGame game = null;

            TestXNAGame.Start( "Mesh.TestSerialize",
                delegate
                {
                    game = TestXNAGame.Instance;


                    GameFile file = new GameFile( System.Windows.Forms.Application.StartupPath + @"\Content\Wall001.dae" );
                    //GameFile file = new GameFile( System.Windows.Forms.Application.StartupPath + @"\Content\NWNTree001.DAE" );
                    //GameFile file = new GameFile( System.Windows.Forms.Application.StartupPath + @"\Content\Terras026.DAE" );
                    //GameFile file = new GameFile( System.Windows.Forms.Application.StartupPath + @"\Content\PakhuisMarktplaats044.DAE" );
                    //GameFile file = new GameFile( System.Windows.Forms.Application.StartupPath + @"\Content\Docks.DAE" );
                    //GameFile file = new GameFile( System.Windows.Forms.Application.StartupPath + @"\Content\Martkplein.DAE" );

                    TWModel model = TWModel.FromColladaFile( game, file );

                    TWWereld wereld = new TWWereld( game );

                    ModelPartStatic part = (ModelPartStatic)model.Parts[ 0 ];

                    WereldMesh mesh = new WereldMesh( wereld );
                    mesh.VertexDeclaration = part.VertexDeclaration;
                    mesh.VertexStride = part.VertexStride;
                    mesh.VertexBuffer = part.VertexBuffer;
                    mesh.IndexBuffer = part.IndexBuffer;
                    mesh.VertexCount = part.VertexCount;
                    mesh.TriangleCount = part.TriangleCount;


                    System.Xml.XmlDocument doc = TWXmlNode.CreateXmlDocument();

                    TWXmlNode meshNode = new TWXmlNode( doc, "Mesh" );
                    mesh.SaveToXml( meshNode );

                    System.IO.FileStream fs = new System.IO.FileStream(
                        System.Windows.Forms.Application.StartupPath + @"\UnitTests\Mesh_TestSerialize.xml"
                        , System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Delete );

                    doc.Save( fs );

                    fs.Close();

                    mesh = new WereldMesh( wereld );
                    mesh.LoadFromXml( TWXmlNode.GetRootNodeFromFile(
                         System.Windows.Forms.Application.StartupPath + @"\UnitTests\Mesh_TestSerialize.xml" ) );


                    doc = TWXmlNode.CreateXmlDocument();
                    mesh.SaveToXml( new TWXmlNode( doc, "Mesh" ) );
                    fs = new System.IO.FileStream(
                        System.Windows.Forms.Application.StartupPath + @"\UnitTests\Mesh_TestSerialize2.xml"
                        , System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Delete );

                    doc.Save( fs );

                    fs.Close();
                },
                delegate
                {
                } );
        }

    }
}
