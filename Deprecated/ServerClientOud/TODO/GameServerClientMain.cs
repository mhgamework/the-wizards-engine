using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.Game3DPlay.Core;
using MHGameWork.TheWizards.ServerClient.Engine;

namespace MHGameWork.TheWizards.ServerClient
{
    public class GameServerClientMain : ServerClientMainOud
    {
        public ServerClientGame serverClientGame;
        public Editor.WorldEditorOud worldEditor;
        public ModeConnectToServer modeConnectToServer;
        public ModeLogin modeLogin;
        public ModeLoading modeLoading;


        public IGameObjectOud activeGameMode;



        public GameServerClientMain()
            : base()
        {
            serverClientGame = new ServerClientGame( this );
            worldEditor = new MHGameWork.TheWizards.ServerClient.Editor.WorldEditorOud( this );
            modeConnectToServer = new ModeConnectToServer( this );
            modeLogin = new ModeLogin( this );
            modeLoading = new ModeLoading( this );

            activeGameMode = serverClientGame;



                       
            

        }


        public override void Dispose()
        {
            string filename = System.Windows.Forms.Application.StartupPath + @"\SavedData\GameFiles.xml";


            GameFileManager.SaveToDisk( filename );

            filename = System.Windows.Forms.Application.StartupPath + @"\SavedData\WereldTerrains.xml";

            Wereld.TerrainManager.SaveToDisk( filename );

            base.Dispose();

        }

        public override void Initialize()
        {
            base.Initialize();

            string filename = System.Windows.Forms.Application.StartupPath + @"\SavedData\GameFiles.xml";

            if ( System.IO.File.Exists( filename ) )
                GameFileManager.LoadFromDisk( filename );

            filename = System.Windows.Forms.Application.StartupPath + @"\SavedData\WereldTerrains.xml";
            if ( System.IO.File.Exists( filename ) )
                Wereld.TerrainManager.LoadFromDisk( filename );




            XNAGame.Graphics.PreferredBackBufferWidth = Settings.ResolutionWidth;
            XNAGame.Graphics.PreferredBackBufferHeight = Settings.ResolutionHeight;
            XNAGame.Graphics.IsFullScreen = Settings.Fullscreen;

            XNAGame.Graphics.ApplyChanges();





            activeGameMode = modeConnectToServer;

            modeConnectToServer.Completed += new EventHandler( modeConnectToServer_Completed );

            modeConnectToServer.Initialize();

            modeConnectToServer.StartGameMode();



            //serverClientGame.Initialize();
            //worldEditor.Initialize();
        }

        void modeConnectToServer_Completed( object sender, EventArgs e )
        {
            activeGameMode = modeLogin;

            modeLogin.Completed += new EventHandler( modeLogin_Completed );


            modeLogin.Initialize();

            modeLogin.StartGameMode();
        }

        void modeLogin_Completed( object sender, EventArgs e )
        {
            activeGameMode = modeLoading;

            modeLoading.Completed += new EventHandler( modeLoading_Completed );

            modeLoading.Initialize();
            modeLoading.StartGameMode();

        }

        void modeLoading_Completed( object sender, EventArgs e )
        {
            activeGameMode = serverClientGame;

            serverClientGame.Initialize();
            //worldEditor.Initialize();
        }



        public override void OnRender( object sender, MHGameWork.Game3DPlay.Core.Elements.RenderEventArgs e )
        {
            activeGameMode.Render();
        }


        public override void OnLoad( object sender, MHGameWork.Game3DPlay.Core.Elements.LoadEventArgs e )
        {
            base.OnLoad( sender, e );
            //serverClientGame.LoadGraphicsContent();
            //worldEditor.LoadGraphicsContent();

        }

        public override void OnUnload( object sender, MHGameWork.Game3DPlay.Core.Elements.LoadEventArgs e )
        {
            base.OnUnload( sender, e );
        }

        public override void OnProcess( object sender, MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            base.OnProcess( sender, e );
            activeGameMode.Process( e );

            if ( e.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.F2 ) )
            {
                if ( activeGameMode == serverClientGame )
                {
                    activeGameMode = worldEditor;
                    worldEditor.Initialize();
                    worldEditor.LoadGraphicsContent();
                }
                else if ( activeGameMode == worldEditor )
                {
                    activeGameMode = serverClientGame;
                }

            }

        }

        public override void OnTick( object sender, MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        {
            base.OnTick( sender, e );
            activeGameMode.Tick( e );

        }
    }
}
