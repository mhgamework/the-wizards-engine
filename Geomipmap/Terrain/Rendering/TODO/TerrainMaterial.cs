using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.Terrain.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Terrain.Rendering
{
    /// <summary>
    /// Parts where removed here, this is not to be used ,go look in the project's history
    /// </summary>
    public class TerrainMaterial : IDisposable
    {
        IXNAGame game;
        private List<TerrainBlock> batchedBlocks = new List<TerrainBlock>();
        private MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainGeomipmapRenderData terrainGeomipmapRenderData;


        private List<TerrainShaderNew> shadersNew;


        // Not sure this should be here

        public List<TerrainTexture> Textures = new List<TerrainTexture>();

        public List<TWTexture> Weightmaps = new List<TWTexture>();

        public TerrainMaterial(IXNAGame _game, MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainGeomipmapRenderData terrainGeomipmapRenderData )
        {
            game = _game;
            this.terrainGeomipmapRenderData = terrainGeomipmapRenderData;
            
            //engine = nEngine;
            //terrain = nTerrain;
        }


        public void BatchBlock( TerrainBlock block )
        {
            batchedBlocks.Add( block );
        }

        public void Render( XNAGame game )
        {
            //
            //TODO : !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            ///
            //


            if ( batchedBlocks.Count == 0 ) return;
            //if ( effects == null ) return;

            //this occurs when this is a material without textures





            game.GraphicsDevice.RenderState.SourceBlend = Blend.One;
            game.GraphicsDevice.RenderState.DestinationBlend = Blend.One;

            game.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add;
            game.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.Always;

            game.GraphicsDevice.RenderState.AlphaBlendEnable = false;





            batchedBlocks.Clear();
        }


        public void RenderTerrainSpecialTemp( XNAGame game, BasicShader shader )
        {


            if ( batchedBlocks.Count == 0 ) return;
            //if ( effects == null ) return;



            shader.SetParameter( "g_matWorld", terrainGeomipmapRenderData.WorldMatrix );

            Matrix transpose, inverseTranspose;
            Matrix transform = terrainGeomipmapRenderData.WorldMatrix;
            Matrix.Transpose( ref transform, out transpose );
            Matrix.Invert( ref transpose, out inverseTranspose );
            shader.SetParameter( "g_matWorldIT", inverseTranspose );
            shader.effect.CommitChanges();



            for ( int iBlock = 0; iBlock < batchedBlocks.Count; iBlock++ )
            {
                DrawBlockPrimitives( batchedBlocks[ iBlock ] );
            }


            batchedBlocks.Clear();
        }



        public void RenderNew( XNAGame game, TerrainShaderNew baseShader )
        {
            //
            //TODO : !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            ///
            //


            if ( batchedBlocks.Count == 0 ) return;
            //if ( effects == null ) return;
            //if ( shadersNew == null || game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.L ) ) LoadNew( baseShader );

            //this occurs when this is a material without textures
            if ( shadersNew == null ) return;





            game.GraphicsDevice.RenderState.SourceBlend = Blend.One;
            game.GraphicsDevice.RenderState.DestinationBlend = Blend.One;

            game.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add;
            game.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.Always;

            game.GraphicsDevice.RenderState.AlphaBlendEnable = false;







            TerrainShaderNew iEffect;

            for ( int i = 0; i < shadersNew.Count; i++ )
            {
                iEffect = shadersNew[ i ];


                if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.I ) )
                {
                    iEffect.Shader.SetTechnique( "DrawWeightmapTexcoordsPreprocessed" );

                }
                if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.O ) )
                {
                    iEffect.Shader.SetTechnique( "DrawWeightmapPreprocessed" );
                }
                if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.U ) )
                {
                    iEffect.Shader.SetTechnique( "DrawTexturedPreprocessed" );
                }

                //iEffect.ViewProjection = game.Camera.ViewProjection;



                iEffect.Shader.Effect.Begin();

                for ( int iPass = 0; iPass < iEffect.Shader.Effect.CurrentTechnique.Passes.Count; iPass++ )
                {
                    EffectPass pass = iEffect.Shader.Effect.CurrentTechnique.Passes[ iPass ];


                    pass.Begin();


                    /*if ( ServerClientMainOud.instance.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.W ) )
                    {

                        if ( engine.XNAGame.GraphicsDevice.RenderState.FillMode == FillMode.Solid )
                        { engine.XNAGame.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame; }
                        else { engine.XNAGame.GraphicsDevice.RenderState.FillMode = FillMode.Solid; }
                    }*/

                    for ( int iBlock = 0; iBlock < batchedBlocks.Count; iBlock++ )
                    {
                        DrawBlockPrimitives( batchedBlocks[ iBlock ] );
                    }

                    pass.End();
                }

                iEffect.Shader.Effect.End();



                game.GraphicsDevice.RenderState.AlphaBlendEnable = true;

            }



            batchedBlocks.Clear();
        }


        protected void DrawBlockPrimitives( TerrainBlock block )
        {
            if ( block.VertexBuffer == null || block.IndexBuffer == null ) return;
            throw new NotImplementedException();
            //game.GraphicsDevice.Vertices[ 0 ].SetSource( block.VertexBuffer, 0, XNAGeoMipMap.VertexMultitextured.SizeInBytes );
            //game.GraphicsDevice.Indices = block.IndexBuffer;


            //TODO: make TotalTriangles?
            //TODO:  game.GraphicsDevice.DrawIndexedPrimitives( PrimitiveType.TriangleList, 0, 0, block.TotalVertices, 0, block.TotalBaseTriangles + block.TotalEdgeTriangles );

            //Terrain.Statistics.DrawCalls += 1;
        }




        public void Load()
        {
            if ( effects != null )
            {
                for ( int i = 0; i < effects.Length; i++ )
                {
                    effects[ i ].Dispose();
                }
                effects = null;
            }




        }
        public void LoadNew( TerrainShaderNew baseShader )
        {
            if ( effects != null )
            {
                for ( int i = 0; i < effects.Length; i++ )
                {
                    effects[ i ].Dispose();
                }
                effects = null;
            }

            TerrainShaderNew iEffect = null;

            if ( Textures.Count == 0 ) return;

            shadersNew = new List<TerrainShaderNew>();
            //effects = new MHGameWork.TheWizards.ServerClient.Engine.ShaderEffect[ (int)Math.Ceiling( (double)Textures.Count / 4 ) ];
            paramsWorldViewProjection = new EffectParameter[ (int)( Textures.Count / 4 ) + 1 ];

            for ( int iTex = 0; iTex < Textures.Count; iTex += 4 )
            {
                //iEffect = new MHGameWork.TheWizards.ServerClient.Engine.ShaderEffect( terrain.Engine, terrain.BaseTerrain.Content.RootDirectory + @"\Content\TerrainEditor.fx" );
                iEffect = baseShader.Clone();

                iEffect.World = terrainGeomipmapRenderData.WorldMatrix;

                //iEffect.Effect.Parameters[ "WeightMap" ].SetValue( terrain.ViewWeightmaps[ iTex >> 2 ] );
                iEffect.Shader.SetParameter( "WeightMap1", Weightmaps[ iTex >> 2 ] );

                if ( Textures.Count > iTex + 0 )
                    iEffect.Shader.SetParameter( "Texture1", Textures[ iTex + 0 ].DiffuseMap );
                if ( Textures.Count > iTex + 1 )
                    iEffect.Shader.SetParameter( "Texture2", Textures[ iTex + 1 ].DiffuseMap );
                if ( Textures.Count > iTex + 2 )
                    iEffect.Shader.SetParameter( "Texture3", Textures[ iTex + 2 ].DiffuseMap );
                if ( Textures.Count > iTex + 3 )
                    iEffect.Shader.SetParameter( "Texture4", Textures[ iTex + 3 ].DiffuseMap );

                //paramsWorldViewProjection[ iTex >> 2 ] = iEffect.Effect.Parameters[ "WorldViewProjection" ];

                iEffect.Shader.SetTechnique( "DrawTexturedPreprocessedShadowed" );
                shadersNew.Add( iEffect );
                //effects[ iTex >> 2 ] = iEffect;
            }







        }





        #region IDisposable Members

        public void Dispose()
        {
            if ( effects != null )
            {
                for ( int i = 0; i < effects.Length; i++ )
                {
                    effects[ i ].Dispose();
                }

            }


            effects = null;

        }

        #endregion
























































        //private Terrain terrain;


        private Engine.ShaderEffect[] effects;



        EffectParameter[] paramsWorldViewProjection;


        //private Engine.GameFileOud effectFile;

        //public Engine.GameFileOud EffectFile
        //{
        //    get { return effectFile; }
        //    set { effectFile = value; }
        //}



        private Matrix worldMatrix;

        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set
            {
                worldMatrix = value;
                if ( effects != null )
                    for ( int i = 0; i < effects.Length; i++ )
                    {
                        effects[ i ].Effect.Parameters[ "World" ].SetValue( value );
                    }
            }
        }




    }
}