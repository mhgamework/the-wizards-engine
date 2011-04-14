using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class TerrainMaterial : IDisposable
    {
        private TerrainOud terrainOud;

        private Engine.ShaderEffect[] effects;

        private List<TerrainTexture> textures = new List<TerrainTexture>();

        public List<TerrainTexture> Textures
        {
            get { return textures; }
            set { textures = value; }
        }

        //EffectParameter paramWorld;
        EffectParameter[] paramsWorldViewProjection;

        //private Engine.Texture texture1;

        //public Engine.Texture Texture1
        //{
        //    get { return texture1; }
        //    set
        //    {
        //        texture1 = value;
        //        effect.Effect.Parameters[ "Texture1" ].SetValue( value.XNATexture );
        //    }
        //}
        //private Engine.Texture texture2;
        //public Engine.Texture Texture2
        //{
        //    get { return texture1; }
        //    set
        //    {
        //        texture1 = value;
        //        effect.Effect.Parameters[ "Texture2" ].SetValue( value.XNATexture );
        //    }
        //}
        //private Engine.Texture texture3;
        //public Engine.Texture Texture3
        //{
        //    get { return texture1; }
        //    set
        //    {
        //        texture1 = value;
        //        effect.Effect.Parameters[ "Texture3" ].SetValue( value.XNATexture );
        //    }
        //}
        //private Engine.Texture texture4;
        //public Engine.Texture Texture4
        //{
        //    get { return texture1; }
        //    set
        //    {
        //        texture1 = value;
        //        effect.Effect.Parameters[ "Texture4" ].SetValue( value.XNATexture );
        //    }
        //}



        private List<Editor.TerrainBlock> batchedBlocks = new List<Editor.TerrainBlock>();


        private List<TerrainBlock> blocks = new List<TerrainBlock>();

        /// <summary>
        /// At the moment only used for preprocessing and editor. Contains the list of blocks that use this material.
        /// </summary>
        public List<TerrainBlock> Blocks
        {
            get { return blocks; }
            set { blocks = value; }
        }

        public TerrainMaterial( TerrainOud nTerrainOud )
        {
            terrainOud = nTerrainOud;
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
                //TODO: iEffect = new MHGameWork.TheWizards.ServerClient.Engine.ShaderEffect( terrainOud.Engine, terrainOud.BaseTerrain.Content.RootDirectory + @"\Content\TerrainEditor.fx" );



                iEffect.Effect.CurrentTechnique = iEffect.Effect.Techniques[ "TexturedEditor" ];
                iEffect.Effect.Parameters[ "World" ].SetValue( terrainOud.WorldMatrix );
                iEffect.Effect.Parameters[ "WeightMap" ].SetValue( terrainOud.ViewWeightmaps[ iTex >> 2 ] );

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

        public void BatchBlock( Editor.TerrainBlock block )
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

            terrainOud.BaseTerrain.Device.RenderState.SourceBlend = Blend.One;
            terrainOud.BaseTerrain.Device.RenderState.DestinationBlend = Blend.One;

            terrainOud.BaseTerrain.Device.RenderState.BlendFunction = BlendFunction.Add;
            terrainOud.BaseTerrain.Device.RenderState.AlphaFunction = CompareFunction.Always;

            terrainOud.BaseTerrain.Device.RenderState.AlphaBlendEnable = false;






            //editorEffect.Effect.CurrentTechnique = editorEffect.Effect.Techniques[ "TexturedEditor" ];

            int visibleTriangles;
            Engine.ShaderEffect iEffect;

            for ( int i = 0; i < effects.Length; i++ )
            {

                paramsWorldViewProjection[ i ].SetValue(
                    terrainOud.WorldMatrix //* Matrix.CreateTranslation( 0, blocks[ 0 ].BaseBlock.BlockNumX, 0 )
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

                        if ( terrainOud.Device.RenderState.FillMode == FillMode.Solid )
                        { terrainOud.Device.RenderState.FillMode = FillMode.WireFrame; }
                        else { terrainOud.Device.RenderState.FillMode = FillMode.Solid; }
                    }

                    for ( int iBlock = 0; iBlock < batchedBlocks.Count; iBlock++ )
                    {
                        visibleTriangles += DrawBlock( batchedBlocks[ iBlock ] );
                    }


                    pass.End();
                }

                iEffect.Effect.End();


                //}

                terrainOud.BaseTerrain.Device.RenderState.AlphaBlendEnable = true;

            }



            batchedBlocks.Clear();
        }

        protected int DrawBlock( Editor.TerrainBlock block )
        {

            if ( block.ViewVertexBuffer == null || block.BaseBlock.IndexBuffer == null ) return 0;
            int totalTriangles = block.BaseBlock.TotalBaseTriangles + block.BaseBlock.TotalEdgeTriangles;

            if ( totalTriangles <= 0 )
                return 0;
            //if ( x > 400 ) return 0;
            terrainOud.Device.Vertices[ 0 ].SetSource( block.ViewVertexBuffer, 0, XNAGeoMipMap.VertexMultitextured.SizeInBytes );
            terrainOud.Device.Indices = block.BaseBlock.IndexBuffer;
            /*device.Textures[ 2 ] = terrain.GetTextureOld( 0 );
            device.Textures[ 3 ] = terrain.GetTextureOld( 1 );
            device.Textures[ 4 ] = terrain.GetTextureOld( 2 );
            device.Textures[ 5 ] = terrain.GetTextureOld( 3 );*/


            terrainOud.Device.DrawIndexedPrimitives( PrimitiveType.TriangleList, 0, 0, block.BaseBlock.TotalVertexes, 0, totalTriangles );


            //Terrain.Statistics.DrawCalls += 1;

            return totalTriangles;
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
    }
}
