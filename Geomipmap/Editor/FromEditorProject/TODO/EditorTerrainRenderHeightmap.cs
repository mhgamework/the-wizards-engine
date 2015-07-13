using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class EditorTerrainRenderHeightmap : IDisposable
    {
        IndexBuffer indexBuffer;
        VertexBuffer vertexBuffer;
        private int numVertices;
        private int numPrimitives;
        VertexDeclaration decl;

        public Texture2D HeightMapTexture;

        private EditorTerrainHeightMapShader shader;

        private WizardsEditor editor;

        public EditorTerrainRenderHeightmap( WizardsEditor _editor )
        {
            editor = _editor;
        }

        public void Initialize( IXNAGame game, TerrainFullData fullData )
        {
            numVertices = ( fullData.SizeX + 1 ) * ( fullData.SizeZ + 1 );
            VertexPositionTexture[] vertices;

            vertices = new VertexPositionTexture[ numVertices ];

            for ( int ix = 0; ix < fullData.SizeX + 1; ix++ )
            {
                for ( int iz = 0; iz < fullData.SizeZ + 1; iz++ )
                {
                    VertexPositionTexture v1 = new VertexPositionTexture();
                    v1.Position = new Microsoft.Xna.Framework.Vector3( ix, 0, iz );
                    v1.TextureCoordinate = new Microsoft.Xna.Framework.Vector2( (float)ix / fullData.SizeX, (float)iz / fullData.SizeZ );

                    vertices[ ix * ( fullData.SizeX + 1 ) + iz ] = v1;



                    //vertices[ ( ix * ( fullData.SizeX + 1 ) + iz ) * 6 + 0 ] = v1;
                    //vertices[ ( ix * ( fullData.SizeX + 1 ) + iz ) * 6 + 1 ] = v3;
                    //vertices[ ( ix * ( fullData.SizeX + 1 ) + iz ) * 6 + 2 ] = v2;
                    //vertices[ ( ix * ( fullData.SizeX + 1 ) + iz ) * 6 + 3 ] = v2;
                    //vertices[ ( ix * ( fullData.SizeX + 1 ) + iz ) * 6 + 4 ] = v3;
                    //vertices[ ( ix * ( fullData.SizeX + 1 ) + iz ) * 6 + 5 ] = v4;
                }
            }

            short[] indices = new short[ fullData.SizeX * fullData.SizeZ * 2 * 3 ];
            numPrimitives = fullData.SizeX * fullData.SizeZ * 2;
            for ( int ix = 0; ix < fullData.SizeX; ix++ )
            {
                for ( int iz = 0; iz < fullData.SizeZ; iz++ )
                {
                    indices[ ( ix * ( fullData.SizeX ) + iz ) * 3 * 2 + 0 ] = (short)( ix * ( fullData.SizeX + 1 ) + iz );
                    indices[ ( ix * ( fullData.SizeX ) + iz ) * 3 * 2 + 1 ] = (short)( ix * ( fullData.SizeX + 1 ) + iz + 1 );
                    indices[ ( ix * ( fullData.SizeX  ) + iz ) * 3 * 2 + 2 ] = (short)( ( ix + 1 ) * ( fullData.SizeX + 1 ) + iz );

                    indices[ ( ix * ( fullData.SizeX ) + iz ) * 3 * 2 + 3 ] = (short)( ( ix + 1 ) * ( fullData.SizeX + 1 ) + iz );
                    indices[ ( ix * ( fullData.SizeX ) + iz ) * 3 * 2 + 4 ] = (short)( ix * ( fullData.SizeX + 1 ) + iz + 1 );
                    indices[ ( ix * ( fullData.SizeX ) + iz ) * 3 * 2 + 5 ] = (short)( ( ix + 1 ) * ( fullData.SizeX + 1 ) + iz + 1 );
                }
            }


            vertexBuffer = new VertexBuffer( game.GraphicsDevice, typeof( VertexPositionTexture ), numVertices, BufferUsage.None );
            vertexBuffer.SetData( vertices );

            indexBuffer = new IndexBuffer( game.GraphicsDevice, typeof( short ), indices.Length, BufferUsage.None );
            indexBuffer.SetData( indices );


            decl = new VertexDeclaration( game.GraphicsDevice, VertexPositionTexture.VertexElements );
            shader = EditorTerrainHeightMapShader.CreateFromEditor( game, editor );

            shader.World = Matrix.CreateTranslation( fullData.Position );


            HeightMapTexture = new Texture2D( game.GraphicsDevice, fullData.SizeX + 1, fullData.SizeZ + 1, 0, TextureUsage.None, SurfaceFormat.Single );
            float[] heights = new float[ numVertices ];
            for ( int ix = 0; ix < fullData.SizeX+1; ix++ )
            {
                for (int iz = 0; iz < fullData.SizeZ+1; iz++)
                {
                    heights[ iz * ( fullData.SizeZ + 1 ) + ix ] = fullData.HeightMap[ ix, iz ];
                }
            }

            HeightMapTexture.SetData( heights );
            shader.effect.Parameters[ "displacementMap" ].SetValue( HeightMapTexture );

            TextureCreationParameters param = TextureCreationParameters.Default;
            //param.MipFilter = FilterOptions.Triangle;
            //param.MipLevels = 10;
            //param.Filter = FilterOptions.Triangle;

            TWTexture gridTileTexture = TWTexture.FromImageFile( game, editor.Files.TerrainGridTexture,param );
            shader.effect.Parameters[ "gridTileTexture" ].SetValue( gridTileTexture.XnaTexture );

            shader.SetTechnique( "DrawGrid" );

            shader.MaxHeight = 3;
        }

        public void Render( IXNAGame game )
        {
           
            //shader.World = Matrix.Identity;
            shader.ViewProjection = game.Camera.ViewProjection;
            game.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
            shader.RenderMultipass( RenderPrimitives );

            game.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
            shader.RenderMultipass( RenderPrimitives );
        }

        private void RenderPrimitives()
        {
            vertexBuffer.GraphicsDevice.Vertices[ 0 ].SetSource( vertexBuffer, 0, VertexPositionTexture.SizeInBytes );
            vertexBuffer.GraphicsDevice.VertexDeclaration = decl;
            vertexBuffer.GraphicsDevice.Indices = indexBuffer;

            vertexBuffer.GraphicsDevice.DrawIndexedPrimitives( PrimitiveType.TriangleList, 0, 0, numVertices, 0, numPrimitives );
        }

        #region IDisposable Members

        public void Dispose()
        {
            if ( vertexBuffer != null ) vertexBuffer.Dispose();
            if ( indexBuffer != null ) indexBuffer.Dispose();
            if ( decl != null ) decl.Dispose();
            if ( shader != null ) shader.effect.Dispose();
            if ( HeightMapTexture != null ) HeightMapTexture.Dispose();
            vertexBuffer = null;
            indexBuffer = null;
            decl = null;

            shader = null;
            HeightMapTexture = null;
        }

        #endregion
    }
}
