using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.Terrain.Rendering;
using MHGameWork.TheWizards.ServerClient.TWClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerrainManagerService = MHGameWork.TheWizards.ServerClient.Terrain.TerrainManagerService;

namespace MHGameWork.TheWizards.ServerClient.Terrain.Rendering
{
    public class TerrainCSMRendererService : Database.IGameService, CascadedShadowMaps.ICSMRenderer
    {
        public TheWizards.Database.Database Database;
        public TerrainManagerService TerrainManager;
        //public ServerClientMain Main;

        private List<MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainGeomipmapRenderData> terrainRenderDatas = new List<MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainGeomipmapRenderData>();
        private CascadedShadowMaps.CSMRendererService csmRendererService;

        private TerrainShaderNew terrainShader;
        private EffectPool effectPool;


        public TerrainCSMRendererService( TheWizards.Database.Database _database )
        {
            Database = _database;
            //Main = Database.FindService<RendererService>().Main;
            TerrainManager = Database.FindService<TerrainManagerService>();
            csmRendererService = Database.FindService<CascadedShadowMaps.CSMRendererService>();
        }

        public void Initialize( XNAGame game )
        {
            if ( terrainShader != null ) terrainShader.Dispose();
            if ( effectPool != null ) effectPool.Dispose();
            effectPool = new EffectPool();
            terrainShader = new TerrainShaderNew( game, effectPool );

            for ( int i = 0; i < TerrainManager.Terrains.Count; i++ )
            {
                TaggedTerrain terrain = TerrainManager.Terrains[ i ];

                TerrainGeomipmapRenderData terr = new TerrainGeomipmapRenderData( game, terrain, TerrainManager );


                terr.CreateBlocksAndQuadtree( new LoadingTask() );



                TerrainPreprocesser tp = new TerrainPreprocesser( terr );
                tp.PreProcessTask( new LoadingTask() );


                //TerrainMaterial mat = new TerrainMaterial( terr, Main );
                //TerrainTexture tex = new TerrainTexture();
                //tex.DiffuseMap = new TWTexture();
                //mat.Textures.Add( tex );
                //terr.Materials.Add( mat );

                //mat.Weightmaps.Add( new TWTexture() );

                //// create blocks
                //for ( int x = 0; x < terr.NumBlocksX; x++ )
                //{
                //    for ( int z = 0; z < terr.NumBlocksZ; z++ )
                //    {
                //        terr.GetBlock( x, z ).Material = mat;
                //    }
                //}

                terr.LoadMaterialsTaskNew( null, terrainShader );

                
                //terr.LoadMaterialsTask( null );


                // create blocks
                for ( int x = 0; x < terr.NumBlocksX; x++ )
                {

                    for ( int z = 0; z < terr.NumBlocksZ; z++ )
                    {
                        terr.LoadBlockTask( terr.GetBlock( x, z ), null );
                    }
                }

                tp.ReadQuadtreeBoundingBoxes( terr.QuadTree );

                terrainRenderDatas.Add( terr );
            }
        }

        public void Render( XNAGame game )
        {
            for ( int i = 0; i < terrainRenderDatas.Count; i++ )
            {
                MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainGeomipmapRenderData data = terrainRenderDatas[ i ];
                game.GraphicsDevice.RenderState.FillMode = Microsoft.Xna.Framework.Graphics.FillMode.Solid;
                data.RenderTerrain( game );

                //game.GraphicsDevice.RenderState.FillMode = Microsoft.Xna.Framework.Graphics.FillMode.WireFrame;
                //data.RenderTerrain( game );
            }
        }

        public void Update( XNAGame game )
        {
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.R ) )
            {

                if ( effectPool != null ) effectPool.Dispose();
                effectPool = new EffectPool();
                terrainShader.Dispose();
                terrainShader = new TerrainShaderNew( game, effectPool );

                for ( int i = 0; i < terrainRenderDatas.Count; i++ )
                {
                    MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainGeomipmapRenderData data = terrainRenderDatas[ i ];

                    for ( int iMat = 0; iMat < data.Materials.Count; iMat++ )
                    {
                        data.Materials[ iMat ].LoadNew( terrainShader );
                    }

                }
            }
            for ( int i = 0; i < terrainRenderDatas.Count; i++ )
            {
                MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainGeomipmapRenderData data = terrainRenderDatas[ i ];
                if ( !game.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Y ) ) data.UpdateTerrain( game.Camera );

            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            //TODO
        }

        #endregion

        #region ICSMRenderer Members


        public void RenderDepth( XNAGame game, BasicShader depthShader )
        {
            for ( int i = 0; i < terrainRenderDatas.Count; i++ )
            {
                MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainGeomipmapRenderData data = terrainRenderDatas[ i ];
                game.GraphicsDevice.RenderState.FillMode = Microsoft.Xna.Framework.Graphics.FillMode.Solid;
                data.RenderTerrainSpecialTemp( game, depthShader );
                //game.GraphicsDevice.RenderState.FillMode = Microsoft.Xna.Framework.Graphics.FillMode.WireFrame;
                //data.RenderTerrain( game );
            }
            //throw new Exception( "The method or operation is not implemented." );
        }

        public void RenderNormal( XNAGame game, BasicShader normalShader )
        {

            terrainShader.ViewProjection = game.Camera.ViewProjection;
            terrainShader.ViewInverse = game.Camera.ViewInverse;
            int width = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int height = game.GraphicsDevice.PresentationParameters.BackBufferHeight;
            terrainShader.BackbufferSize = new Vector2( width, height );

            if ( csmRendererService != null )
            {
                terrainShader.ShadowOcclusionTexture = csmRendererService.GetShadowOcclusionTexture();

                terrainShader.LightDirection = csmRendererService.light.Direction;
                terrainShader.LightColor = csmRendererService.light.Color;
            }
            terrainShader.AmbientColor = new Vector4( 0.3f, 0.3f, 0.3f, 1 );

            for ( int i = 0; i < terrainRenderDatas.Count; i++ )
            {
                MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainGeomipmapRenderData data = terrainRenderDatas[ i ];
                game.GraphicsDevice.RenderState.FillMode = Microsoft.Xna.Framework.Graphics.FillMode.Solid;
                //game.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;

                //data.RenderTerrainSpecialTemp( game, normalShader );

                data.RenderTerrainNew( game, terrainShader );

                //game.GraphicsDevice.RenderState.normalShader = Microsoft.Xna.Framework.Graphics.FillMode.WireFrame;
                //data.RenderTerrain( game );
            }
            //throw new Exception( "The method or operation is not implemented." );
            game.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
        }

        public void RenderShadowMap( XNAGame game, BasicShader shadowMapShader )
        {
            for ( int i = 0; i < terrainRenderDatas.Count; i++ )
            {
                MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainGeomipmapRenderData data = terrainRenderDatas[ i ];
                game.GraphicsDevice.RenderState.FillMode = Microsoft.Xna.Framework.Graphics.FillMode.Solid;
                data.RenderTerrainSpecialTemp( game, shadowMapShader );
                //game.GraphicsDevice.RenderState.FillMode = Microsoft.Xna.Framework.Graphics.FillMode.WireFrame;
                //data.RenderTerrain( game );
            }
            //throw new Exception( "The method or operation is not implemented." );
        }

        #endregion
    }
}