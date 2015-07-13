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
    public class EditorTerrainRenderTextured : IDisposable
    {
        IndexBuffer indexBuffer;
        VertexBuffer vertexBuffer;
        private int numVertices;
        private int numPrimitives;
        VertexDeclaration decl;

        public Texture2D HeightMapTexture;
        public List<Texture2D> WeightMapTextures;

        private List<EditorTerrainHeightMapShader> shaders;

        private WizardsEditor editor;

        public EditorTerrainRenderTextured( WizardsEditor _editor )
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
                    v1.TextureCoordinate = new Microsoft.Xna.Framework.Vector2( (float)ix / ( fullData.SizeX + 1 ), (float)iz / ( fullData.SizeZ + 1 ) );

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
                    indices[ ( ix * ( fullData.SizeX ) + iz ) * 3 * 2 + 2 ] = (short)( ( ix + 1 ) * ( fullData.SizeX + 1 ) + iz );

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



            HeightMapTexture = new Texture2D( game.GraphicsDevice, fullData.SizeX + 1, fullData.SizeZ + 1, 0, TextureUsage.None, SurfaceFormat.Single );
            float[] heights = new float[ numVertices ];
            for ( int ix = 0; ix < fullData.SizeX + 1; ix++ )
            {
                for ( int iz = 0; iz < fullData.SizeZ + 1; iz++ )
                {
                    heights[ iz * ( fullData.SizeZ + 1 ) + ix ] = fullData.HeightMap[ ix, iz ];
                }
            }

            HeightMapTexture.SetData( heights );

            WeightMapTextures = new List<Texture2D>();
            shaders = new List<EditorTerrainHeightMapShader>();

            //if ( fullData.Textures.Count > 0 )  if not textures this still works
            for ( int iWeightMap = 0; iWeightMap < ( fullData.Textures.Count - 1 ) / 4 + 1; iWeightMap++ )
            {
                Texture2D weightMapTex = new Texture2D( game.GraphicsDevice, fullData.SizeX + 1, fullData.SizeZ + 1, 0, TextureUsage.None, SurfaceFormat.Color );
                Color[] weights = new Color[ numVertices ];
                if ( fullData.Textures.Count > 0 )
                    for ( int ix = 0; ix < fullData.SizeX + 1; ix++ )
                    {
                        for ( int iz = 0; iz < fullData.SizeZ + 1; iz++ )
                        {
                            weights[ iz * ( fullData.SizeZ + 1 ) + ix ] = fullData.CalculateWeightMapColor( ix, iz, iWeightMap );
                        }
                    }
                weightMapTex.SetData( weights );
                WeightMapTextures.Add( weightMapTex );



                EditorTerrainHeightMapShader shader;

                shader = EditorTerrainHeightMapShader.CreateFromEditor( game, editor );

                shader.World = Matrix.CreateTranslation( fullData.Position );



                shader.effect.Parameters[ "displacementMap" ].SetValue( HeightMapTexture );

                TWTexture gridTileTexture = TWTexture.FromImageFile( game, editor.Files.TerrainGridTexture );
                shader.effect.Parameters[ "gridTileTexture" ].SetValue( gridTileTexture.XnaTexture );


                shader.effect.Parameters[ "WeightMap1" ].SetValue( weightMapTex );

                TWTexture tex;
                if ( fullData.Textures.Count > iWeightMap * 4 + 0 )
                {
                    tex = TWTexture.FromImageFile( game,
                                                                  new GameFile( fullData.Textures[ iWeightMap * 4 + 0 ].DiffuseTexture ) );
                    shader.effect.Parameters[ "Texture1" ].SetValue( tex.XnaTexture );
                }
                if ( fullData.Textures.Count > iWeightMap * 4 + 1 )
                {
                    tex = TWTexture.FromImageFile( game,
                                                                  new GameFile( fullData.Textures[ iWeightMap * 4 + 1 ].DiffuseTexture ) );
                    shader.effect.Parameters[ "Texture2" ].SetValue( tex.XnaTexture );
                }
                if ( fullData.Textures.Count > iWeightMap * 4 + 2 )
                {
                    tex = TWTexture.FromImageFile( game,
                                                                  new GameFile( fullData.Textures[ iWeightMap * 4 + 2 ].DiffuseTexture ) );
                    shader.effect.Parameters[ "Texture3" ].SetValue( tex.XnaTexture );
                }
                if ( fullData.Textures.Count > iWeightMap * 4 + 3 )
                {
                    tex = TWTexture.FromImageFile( game,
                                                                  new GameFile( fullData.Textures[ iWeightMap * 4 + 3 ].DiffuseTexture ) );
                    shader.effect.Parameters[ "Texture4" ].SetValue( tex.XnaTexture );
                }


                shader.SetTechnique( "DrawHeightMapTextured" );

                shader.MaxHeight = 3;

                shaders.Add( shader );

            }



        }

        public void Render( IXNAGame game )
        {
            game.GraphicsDevice.RenderState.SourceBlend = Blend.One;
            game.GraphicsDevice.RenderState.DestinationBlend = Blend.One;

            game.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add;
            game.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.Always;
            game.GraphicsDevice.RenderState.AlphaBlendEnable = false;


            for ( int i = 0; i < shaders.Count; i++ )
            {
                EditorTerrainHeightMapShader shader = shaders[ i ];
                //shader.World = Matrix.Identity;
                shader.ViewProjection = game.Camera.ViewProjection;
                game.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
                shader.RenderMultipass( RenderPrimitives );

                game.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
                shader.RenderMultipass( RenderPrimitives );
                game.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            }


            game.GraphicsDevice.RenderState.SourceBlend = Blend.One;
            game.GraphicsDevice.RenderState.DestinationBlend = Blend.Zero;

            game.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add;
            game.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.Always;
            game.GraphicsDevice.RenderState.AlphaBlendEnable = false;

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
            if ( shaders != null )
            {
                for ( int i = 0; i < shaders.Count; i++ )
                {
                    EditorTerrainHeightMapShader shader = shaders[ i ];
                    shader.effect.Dispose();
                }
                shaders.Clear();
            }

            if ( HeightMapTexture != null ) HeightMapTexture.Dispose();
            if ( WeightMapTextures != null )
                for ( int i = 0; i < WeightMapTextures.Count; i++ )
                {
                    WeightMapTextures[ i ].Dispose();
                }
            vertexBuffer = null;
            indexBuffer = null;
            decl = null;

            shaders = null;
            HeightMapTexture = null;
            WeightMapTextures.Clear();
            WeightMapTextures = null;
        }

        #endregion
    }
}
