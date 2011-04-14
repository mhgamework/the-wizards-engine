using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.XNAGeoMipMap
{
    public class TerrainMaterial : IDisposable
    {
        //private Terrain terrain;

        private ServerClientMainOud engine;

        private Engine.ShaderEffect[] effects;

        private List<TerrainTexture> textures = new List<TerrainTexture>();

        public List<TerrainTexture> Textures
        {
            get { return textures; }
            set { textures = value; }
        }

        EffectParameter[] paramsWorldViewProjection;

        private List<ITerrainBlockRenderable> batchedBlocks = new List<ITerrainBlockRenderable>();

        private Engine.GameFileOud effectFile;

        public Engine.GameFileOud EffectFile
        {
            get { return effectFile; }
            set { effectFile = value; }
        }

        private List<Engine.Texture> weightmaps;

        public List<Engine.Texture> Weightmaps
        {
            get { return weightmaps; }
            set { weightmaps = value; }
        }

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

        public TerrainMaterial( ServerClientMainOud nEngine )
        {
            engine = nEngine;
            //terrain = nTerrain;
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

            Engine.ShaderEffect iEffect = null;

            if ( textures.Count == 0 ) return;


            effects = new MHGameWork.TheWizards.ServerClient.Engine.ShaderEffect[ (int)Math.Ceiling( (double)textures.Count / 4 ) ];
            paramsWorldViewProjection = new EffectParameter[ (int)( textures.Count / 4 ) + 1 ];

            for ( int iTex = 0; iTex < textures.Count; iTex += 4 )
            {
                //iEffect = new MHGameWork.TheWizards.ServerClient.Engine.ShaderEffect( terrain.Engine, terrain.BaseTerrain.Content.RootDirectory + @"\Content\TerrainEditor.fx" );
                //TODO: iEffect = new MHGameWork.TheWizards.ServerClient.Engine.ShaderEffect( engine, effectFile.GetFullFilename() );


                iEffect.Effect.CurrentTechnique = iEffect.Effect.Techniques[ "TexturedEditor" ];
                iEffect.Effect.Parameters[ "World" ].SetValue( worldMatrix );
                //iEffect.Effect.Parameters[ "WeightMap" ].SetValue( terrain.ViewWeightmaps[ iTex >> 2 ] );
                iEffect.Effect.Parameters[ "WeightMap" ].SetValue( Weightmaps[ iTex >> 2 ].XNATexture );

                if ( textures.Count > iTex + 0 )
                    iEffect.Effect.Parameters[ "Texture1" ].SetValue( textures[ iTex + 0 ].DiffuseMap.XNATexture );
                if ( textures.Count > iTex + 1 )
                    iEffect.Effect.Parameters[ "Texture2" ].SetValue( textures[ iTex + 1 ].DiffuseMap.XNATexture );
                if ( textures.Count > iTex + 2 )
                    iEffect.Effect.Parameters[ "Texture3" ].SetValue( textures[ iTex + 2 ].DiffuseMap.XNATexture );
                if ( textures.Count > iTex + 3 )
                    iEffect.Effect.Parameters[ "Texture4" ].SetValue( textures[ iTex + 3 ].DiffuseMap.XNATexture );

                paramsWorldViewProjection[ iTex >> 2 ] = iEffect.Effect.Parameters[ "WorldViewProjection" ];

                effects[ iTex >> 2 ] = iEffect;
            }






        }

        public void BatchBlock( ITerrainBlockRenderable block )
        {
            batchedBlocks.Add( block );
        }

        public void Render( ServerClientMainOud engine )
        {
            //return;
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //TODO : !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            ///
            //


            //paramWorld.SetValue( terrain.WorldMatrix );
            if ( batchedBlocks.Count == 0 ) return;
            if ( effects == null ) return;


            //terrain.BaseTerrain.Device.RenderState.SourceBlend = Blend.One;
            //terrain.BaseTerrain.Device.RenderState.DestinationBlend = Blend.One;

            engine.XNAGame.GraphicsDevice.RenderState.SourceBlend = Blend.One;
            engine.XNAGame.GraphicsDevice.RenderState.DestinationBlend = Blend.One;

            engine.XNAGame.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add;
            engine.XNAGame.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.Always;

            engine.XNAGame.GraphicsDevice.RenderState.AlphaBlendEnable = false;






            //editorEffect.Effect.CurrentTechnique = editorEffect.Effect.Techniques[ "TexturedEditor" ];

            int visibleTriangles;
            Engine.ShaderEffect iEffect;

            for ( int i = 0; i < effects.Length; i++ )
            {

                paramsWorldViewProjection[ i ].SetValue(
                    worldMatrix //* Matrix.CreateTranslation( 0, blocks[ 0 ].BaseBlock.BlockNumX, 0 )
                    * engine.ActiveCamera.CameraInfo.ViewProjectionMatrix );


                iEffect = effects[ i ];

                // for ( int iWeightMap = 0; iWeightMap < weightMaps.Count; iWeightMap++ )
                //{



                iEffect.Effect.Begin();
                visibleTriangles = 0;

                for ( int iPass = 0; iPass < iEffect.Effect.CurrentTechnique.Passes.Count; iPass++ )
                {
                    EffectPass pass = iEffect.Effect.CurrentTechnique.Passes[ iPass ];

                    //iEffect.Effect.Begin();
                    //editorEffect.Effect.CommitChanges();

                    pass.Begin();


                    if ( ServerClientMainOud.instance.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.W ) )
                    {

                        if ( engine.XNAGame.GraphicsDevice.RenderState.FillMode == FillMode.Solid )
                        { engine.XNAGame.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame; }
                        else { engine.XNAGame.GraphicsDevice.RenderState.FillMode = FillMode.Solid; }
                    }

                    for ( int iBlock = 0; iBlock < batchedBlocks.Count; iBlock++ )
                    {
                        visibleTriangles += batchedBlocks[ iBlock ].DrawPrimitives();
                    }


                    pass.End();
                }

                iEffect.Effect.End();


                //}

                engine.XNAGame.GraphicsDevice.RenderState.AlphaBlendEnable = true;

            }



            batchedBlocks.Clear();
        }

        //protected int DrawBlock( ITerrainBlockRenderable block )
        //{

        //    if ( block.ViewVertexBuffer == null || block.BaseBlock.IndexBuffer == null ) return 0;
        //    int totalTriangles = block.BaseBlock.TotalBaseTriangles + block.BaseBlock.TotalEdgeTriangles;

        //    if ( totalTriangles <= 0 )
        //        return 0;
        //    engine.XNAGame.GraphicsDevice.Vertices[ 0 ].SetSource( block.ViewVertexBuffer, 0, XNAGeoMipMap.VertexMultitextured.SizeInBytes );
        //    engine.XNAGame.GraphicsDevice.Indices = block.BaseBlock.IndexBuffer;


        //    engine.XNAGame.GraphicsDevice.DrawIndexedPrimitives( PrimitiveType.TriangleList, 0, 0, block.BaseBlock.TotalVertexes, 0, totalTriangles );


        //    //Terrain.Statistics.DrawCalls += 1;

        //    return totalTriangles;
        //}


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
    }
}
