using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class EditorModelPart : IRenderPrimitives
    {

        private EditorModel editorModel;

        public EditorModel EditorModel
        {
            get { return editorModel; }
            set { editorModel = value; }
        }



        private Matrix localMatrix;

        /// <summary>
        /// This is the orientation for this modelpart in Object Space
        /// </summary>
        public Matrix LocalMatrix
        {
            get { return localMatrix; }
            set { localMatrix = value; }
        }

      
        private List<TangentVertex> vertices;

        public List<TangentVertex> Vertices
        {
            get { return vertices; }
            set { vertices = value; }
        }

        private List<int> indices;

        public List<int> Indices
        {
            get { return indices; }
            set { indices = value; }
        }




        private BoundingBox boundingBox;

        public BoundingBox BoundingBox
        {
            get { return boundingBox; }
            set { boundingBox = value; }
        }


        public EditorModelPart( EditorModel nModel )
        {
            EditorModel = nModel;
        }


        public void Render()
        {
            if ( Material != null )
                Material.RenderPrimitivesBatched( this );
        }

        void IRenderPrimitives.RenderPrimitives()
        {
            //Note: We could require the graphicsdevice as a parameter, but since the vertexbuffer or indexbuffer
            //        won't render (i think?) on any other device than the one it was created on,
            //        we can safely assume that we are rendering to that graphicsdevice.
            VertexBuffer.GraphicsDevice.Vertices[ 0 ].SetSource( VertexBuffer, 0, VertexStride );
            VertexBuffer.GraphicsDevice.Indices = IndexBuffer;
            VertexBuffer.GraphicsDevice.VertexDeclaration = VertexDeclaration;
            VertexBuffer.GraphicsDevice.DrawIndexedPrimitives( PrimitiveType.TriangleList, 0, 0, VertexCount, 0, TriangleCount );
        }
        public static EditorModelPart FromColladaMeshPart( EditorModel model, ColladaMesh.PrimitiveList meshPart )
        {
            EditorModelPart modelPart = new EditorModelPart( model );

            modelPart.TriangleCount = meshPart.PrimitiveCount;

            List<TangentVertex> vertices = new List<TangentVertex>();
            List<Vector3> positions = new List<Vector3>();
            List<int> indices = new List<int>();
            // Construct data
            vertices.Clear();


            // We have to use int indices here because we often have models
            // with more than 64k triangles (even if that gets optimized later).
            for ( int i = 0; i < modelPart.TriangleCount * 3; i++ )
            {
                if ( i == int.MaxValue ) throw new Exception();

                Vector3 position = meshPart.GetVector3( ColladaMesh.Input.InputSemantics.Position, i ); //GetPosition( i );
                positions.Add( position );
                Vector3 normal = meshPart.GetVector3( ColladaMesh.Input.InputSemantics.Normal, i );


                // Texture Coordinates
                Vector3 texcoord = Vector3.Zero;
                if ( meshPart.ContainsInput( ColladaMesh.Input.InputSemantics.Texcoord, 1 ) )
                {
                    texcoord = meshPart.GetVector3( ColladaMesh.Input.InputSemantics.Texcoord, 1, i );
                    texcoord.Y = 1.0f - texcoord.Y; // V coordinate is inverted in max
                }


                // Tangent
                Vector3 tangent = Vector3.Zero;
                if ( meshPart.ContainsInput( ColladaMesh.Input.InputSemantics.TexTangent, 1 ) )
                {
                    tangent = meshPart.GetVector3( ColladaMesh.Input.InputSemantics.TexTangent, 1, i );
                }

                vertices.Add( new TangentVertex( position, texcoord.X, texcoord.Y, normal, tangent ) );
                indices.Add( i );


            }

            modelPart.vertices = vertices;
            modelPart.indices = indices;

            // Calculate the bounding box
            modelPart.boundingBox = BoundingBox.CreateFromPoints( positions );

            //TODO: Material trick not really ok
            if ( meshPart.Material != null )
                modelPart.Material = model.tempMaterials[ meshPart.Material ];


            return modelPart;
        }

        public void Initialize( IXNAGame game )
        {

            //
            // Create the Vertex buffer and the index buffer
            //

            VertexCount = vertices.Count;
            VertexDeclaration = TangentVertex.CreateVertexDeclaration( game );
            VertexStride = TangentVertex.SizeInBytes;

            // Create the vertex buffer from our vertices.
            VertexBuffer = new VertexBuffer( game.GraphicsDevice, typeof( TangentVertex ), VertexCount, BufferUsage.None );
            VertexBuffer.SetData( vertices.ToArray() );

            //// We only support max. 65535 optimized vertices, which is really a
            //// lot, but more would require a int index buffer (twice as big) and
            //// I have never seen any realtime 3D model that needs more vertices ^^
            //if ( modelPart.VertexCount > ushort.MaxValue )
            //    throw new InvalidOperationException(
            //        "Too much vertices to index, optimize vertices or use " +
            //        "fewer vertices. Vertices=" + vertices.Count +
            //        ", Max Vertices for Index Buffer=" + ushort.MaxValue );

            // Create the index buffer from our indices (Note: While the indices
            // will point only to 16bit (ushort) vertices, we can have a lot
            // more indices in this list than just 65535).
            IndexBuffer = new IndexBuffer( game.GraphicsDevice, typeof( int ), indices.Count, BufferUsage.None );
            IndexBuffer.SetData( indices.ToArray() );

        }

    }
}
