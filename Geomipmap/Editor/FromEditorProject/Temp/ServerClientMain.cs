using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.Terrain;
using MHGameWork.TheWizards.ServerClient.TWClient;
using DiskLoaderService = MHGameWork.TheWizards.ServerClient.Database.DiskLoaderService;
using TerrainRendererService = MHGameWork.TheWizards.ServerClient.Terrain.Rendering.TerrainRendererService;

namespace MHGameWork.TheWizards.ServerClient
{
    public class ServerClientMain
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




        public ServerClientMain()
        {
            xnaGame = new XNAGame();
            xnaGame.InitializeEvent += new EventHandler( xnaGame_InitializeEvent );
            xnaGame.UpdateEvent += new XNAGame.XNAGameLoopEventHandler( xnaGame_UpdateEvent );
            xnaGame.DrawEvent += new XNAGame.XNAGameLoopEventHandler( xnaGame_DrawEvent );
            Files = new ServerClientFiles( Application.StartupPath );

            database = new TheWizards.Database.Database();

            database.AddService( new Database.DiskSerializerService( database, Application.StartupPath + "\\WizardsEditorSave" ) );
            database.AddService( new DiskLoaderService( database ) );


            //database.FindService<DiskLoaderService>().SetSerializationDirectory( dirMain );
            database.AddService( new Database.SettingsService( database, System.Windows.Forms.Application.StartupPath + "\\Settings.xml" ) );

            // Create the renderer service
            RendererService = new MHGameWork.TheWizards.ServerClient.TWClient.RendererService( xnaGame, database );
            database.AddService( RendererService );


            //Sky.SkyRenderer skyRenderer = new Sky.SkyRenderer( database );
            //database.AddService( new CascadedShadowMaps.CSMRendererService( xnaGame, database ) );
            Sky.SkyCSMRenderer skyCSMRenderer = new MHGameWork.TheWizards.ServerClient.Sky.SkyCSMRenderer( database );
            //Water.WaterCSMRenderer waterCSMRenderer = new MHGameWork.TheWizards.ServerClient.Water.WaterCSMRenderer( xnaGame, database, skyCSMRenderer );
            //RendererService.AddIRenderer( waterCSMRenderer );



            // Load all plugins in assembly
            List<IPlugin002> plugins = Editor.PluginLoader.GetPlugins<IPlugin002>( System.Reflection.Assembly.GetExecutingAssembly() );
            for ( int i = 0; i < plugins.Count; i++ )
            {
                IPlugin002 plugin = plugins[ i ];
                plugin.LoadPlugin( database );
            }

            //RendererService.AddIRenderer( new Entity.Rendering.EntityRenderer( database ) );
            /*Database.FindService<CascadedShadowMaps.CSMRendererService>().CSMRenderers.Add( new Sky.SkyCSMRenderer(database) );
            Database.FindService<CascadedShadowMaps.CSMRendererService>().CSMRenderers.Add( new Entity.Rendering.EntityCSMRenderer( database ) );
            Database.FindService<CascadedShadowMaps.CSMRendererService>().CSMRenderers.Add( new Terrain.Rendering.TerrainCSMRendererService( database ) );*/

            /*waterCSMRenderer.CSMRenderers.Add( skyCSMRenderer );
            waterCSMRenderer.CSMRenderers.Add( new Entity.Rendering.EntityCSMRenderer( database ) );
            waterCSMRenderer.CSMRenderers.Add( new Terrain.Rendering.TerrainCSMRendererService( database ) );*/


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
