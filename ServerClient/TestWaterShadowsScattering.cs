using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.Terrain;
using MHGameWork.TheWizards.ServerClient.TWClient;
using DiskLoaderService = MHGameWork.TheWizards.ServerClient.Database.DiskLoaderService;
using TerrainRendererService = MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainRendererService;
using MHGameWork.TheWizards.ServerClient.Editor;
using MHGameWork.TheWizards.ServerClient.Entity.Rendering;
using MHGameWork.TheWizards.ServerClient.Terrain.Rendering;
using MHGameWork.TheWizards.ServerClient.Water;
using MHGameWork.TheWizards.ServerClient.Sky;
using MHGameWork.TheWizards.ServerClient.Database;

namespace MHGameWork.TheWizards.ServerClient
{
    public class TestWaterShadowsScattering : IUnitTestGame
    {
        private XNAGame xnaGame;
        public XNAGame XNAGame { get { return xnaGame; } }

        private TheWizards.Database.Database database;
        public TheWizards.Database.Database Database
        {
            get { return database; }
            //set { database = value; }
        }

        public ServerClientFiles Files;

        public TWClient.RendererService RendererService;




        public TestWaterShadowsScattering()
        {
            xnaGame = new XNAGame();
            xnaGame.InitializeEvent += xnaGame_InitializeEvent;
            xnaGame.UpdateEvent += xnaGame_UpdateEvent;
            xnaGame.DrawEvent += xnaGame_DrawEvent;
            Files = new ServerClientFiles( Application.StartupPath );

            database = new TheWizards.Database.Database();

            database.AddService( new DiskSerializerService( database, Application.StartupPath + "\\WizardsEditorSave" ) );
            database.AddService( new DiskLoaderService( database ) );


            //database.FindService<DiskLoaderService>().SetSerializationDirectory( dirMain );
            database.AddService( new Database.SettingsService( database, System.Windows.Forms.Application.StartupPath + "\\Settings.xml" ) );

            // Create the renderer service
            RendererService = new RendererService( xnaGame, database );
            database.AddService( RendererService );


            //Sky.SkyRenderer skyRenderer = new Sky.SkyRenderer( database );
            //database.AddService( new CascadedShadowMaps.CSMRendererService( xnaGame, database ) );
            SkyCSMRenderer skyCSMRenderer = new MHGameWork.TheWizards.ServerClient.Sky.SkyCSMRenderer( database );
            WaterCSMRenderer waterCSMRenderer = new WaterCSMRenderer( xnaGame, database, skyCSMRenderer );
            RendererService.AddIRenderer( waterCSMRenderer );



            // Load all plugins in assembly
            List<IPlugin002> plugins = PluginLoader.GetPlugins<IPlugin002>( AppDomain.CurrentDomain.GetAssemblies() );
            for ( int i = 0; i < plugins.Count; i++ )
            {
                IPlugin002 plugin = plugins[ i ];
                plugin.LoadPlugin( database );
            }

            //RendererService.AddIRenderer( new Entity.Rendering.EntityRenderer( database ) );
            /*Database.FindService<CascadedShadowMaps.CSMRendererService>().CSMRenderers.Add( new Sky.SkyCSMRenderer(database) );
            Database.FindService<CascadedShadowMaps.CSMRendererService>().CSMRenderers.Add( new Entity.Rendering.EntityCSMRenderer( database ) );
            Database.FindService<CascadedShadowMaps.CSMRendererService>().CSMRenderers.Add( new Terrain.Rendering.TerrainCSMRendererService( database ) );*/

            waterCSMRenderer.CSMRenderers.Add( skyCSMRenderer );
            waterCSMRenderer.CSMRenderers.Add( new EntityCSMRenderer( database ) );
            waterCSMRenderer.CSMRenderers.Add( new TerrainCSMRendererService( database ) );


            ( (SpectaterCamera)xnaGame.Camera ).FarClip = 1000;


            /*

            // Load Terrain plugin
            IPlugin002 plugin;

            plugin = new Terrain.TerrainPlugin();
            plugin.LoadPlugin( database );

            // Load Entities plugin
            */


            //terrainRenderer = new TerrainRendererService( this, database );



            //database.FindService<DiskSerializerService>().LoadFromDisk();





        }

        void xnaGame_DrawEvent()
        {
            RendererService.Render( xnaGame );
            //terrainRenderer.Render( xnaGame );
        }

        void xnaGame_UpdateEvent()
        {
            RendererService.Update( xnaGame );

        }

        void xnaGame_InitializeEvent( object sender, EventArgs e )
        {
            database.FindService<DiskLoaderService>().LoadFromDisk();

            RendererService.Initialize( xnaGame );

        }

        public void Run()
        {
            xnaGame.Run();
        }

        public void Exit()
        {
            xnaGame.Exit();
            xnaGame = null;
        }

    }
}
