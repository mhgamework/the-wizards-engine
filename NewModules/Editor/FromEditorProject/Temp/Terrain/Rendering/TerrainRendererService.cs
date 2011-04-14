using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.Terrain.Rendering;
using MHGameWork.TheWizards.ServerClient.TWClient;
using TerrainManagerService = MHGameWork.TheWizards.ServerClient.Terrain.TerrainManagerService;

namespace MHGameWork.TheWizards.ServerClient.Terrain.Rendering
{
    public class TerrainRendererService : Database.IGameService, IRenderer
    {
        public TheWizards.Database.Database Database;
        public TerrainManagerService TerrainManager;
        //public ServerClientMain Main;

        private List<MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainGeomipmapRenderData> terrainRenderDatas = new List<MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainGeomipmapRenderData>();

        public TerrainRendererService( TheWizards.Database.Database _database )
        {
            Database = _database;
            //Main = Database.FindService<RendererService>().Main;
            TerrainManager = Database.FindService<TerrainManagerService>();
        }

        public void Initialize( XNAGame game )
        {
            for ( int i = 0; i < TerrainManager.Terrains.Count; i++ )
            {
                TaggedTerrain terrain = TerrainManager.Terrains[ i ];

                MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainGeomipmapRenderData terr = new MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainGeomipmapRenderData( game, terrain, TerrainManager );


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


                terr.LoadMaterialsTask( null );


                // create blocks
                for ( int x = 0; x < terr.NumBlocksX; x++ )
                {

                    for ( int z = 0; z < terr.NumBlocksZ; z++ )
                    {
                        terr.LoadBlockTask( terr.GetBlock( x, z ), null );
                    }
                }

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
            for ( int i = 0; i < terrainRenderDatas.Count; i++ )
            {
                MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainGeomipmapRenderData data = terrainRenderDatas[ i ];
                data.UpdateTerrain( game.Camera );
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            //TODO
        }

        #endregion
    }
}