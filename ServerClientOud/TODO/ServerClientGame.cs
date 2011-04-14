using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards;
using MHGameWork;
using MHGameWork.TheWizards.ServerClient.Engine;

namespace MHGameWork.TheWizards.ServerClient
{
    public class ServerClientGame : IGameObjectOud
    {
        public ServerClientMainOud engine;

        //private bool loggingIn;

        public XNAGeoMipMap.SkyBox001 sky;

        XNAGeoMipMap.Water001 water;

        public ServerClientGame( ServerClientMainOud nEngine )
        {
            engine = nEngine;


            GameFileOud model = new GameFileOud( engine.XNAGame.Content.RootDirectory + @"\Content\skybox" );
            GameFileOud shader = new GameFileOud( engine.XNAGame.Content.RootDirectory + @"\Content\Skybox001.fx" );
            GameFileOud texture = new GameFileOud( engine.XNAGame.Content.RootDirectory + @"\Content\Skybox001.dds" );


            sky = new XNAGeoMipMap.SkyBox001( engine, model, shader, texture );

            engine.LoadingManager.AddLoadTaskAdvanced( sky.LoadNormalTask, LoadingTaskType.Normal );






        }

        public void Initialize()
        {
            water = new XNAGeoMipMap.Water001();

            water.Load( engine.XNAGame.Graphics.GraphicsDevice );

            water.World = Matrix.CreateTranslation( -100, 0, -100 ) * Matrix.CreateScale( 100 ) * Matrix.CreateTranslation( 0, 2, 0 );



            //engine.ServerMain.Communication.StartListening();


            engine.SetCamera( engine.GameClient.PlayerCamera );
        }

        public void LoadGraphicsContent()
        {

        }




        public void Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            water.Time += e.Elapsed;
            //water.World = Matrix.CreateTranslation( -100, 0, -100 ) * Matrix.CreateScale( 100 ) * Matrix.CreateTranslation( 0, 2, 0 );

            if ( !engine.LoadingManager.ProcessNextTaskInterval( LoadingTaskType.Normal, 0 ) )
                engine.LoadingManager.ProcessNextTaskInterval( LoadingTaskType.Detail, 1000 );

            engine.LoadingManager.ProcessNextTaskInterval( LoadingTaskType.Unload, 20 );

            if ( e.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.N ) )
            {
                engine.serverDebugRenderer.Enabled = !engine.serverDebugRenderer.Enabled;

            }

            if ( e.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.B ) )
            {
                engine.debugRenderer.Enabled = true;
            }
            if ( e.Keyboard.IsKeyStateUp( Microsoft.Xna.Framework.Input.Keys.B ) )
            {
                engine.debugRenderer.Enabled = false;
            }

            engine.Wereld.Process( e );

            engine.GameClient.Process( e );


            engine.GameClient.PlayerCamera.Process( e );
        }

        public void Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        {
            engine.XNAGame.Window.Title = "The Wizards      FPS: " + engine.FPS.ToString();
            engine.Wereld.Tick( e );


        }



        public void Render()
        {
            if ( engine.GameClient.PlayerCamera != null ) engine.SetCamera( engine.GameClient.PlayerCamera );

            sky.Render();



            engine.Wereld.Render();
            water.Render();
            engine.XNAGame.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
            engine.LineManager3D.Render();
        }

        public GraphicsDevice GraphicsDevice
        { get { return engine.XNAGame.GraphicsDevice; } }





        public static void TestServerClientGame001()
        {
            TestServerClientMain main = null;

            ServerClientGame game = null;

            bool started = false;


            TestServerClientMain.Start( "TestServerClientGame001",
                delegate
                {
                    main = TestServerClientMain.Instance;

                    main.XNAGame.Graphics.PreferredBackBufferWidth = 1280;
                    main.XNAGame.Graphics.PreferredBackBufferHeight = 1024;
                    main.XNAGame.Graphics.ApplyChanges();

                    game = new ServerClientGame( main );

                    game.Initialize();



                },
                delegate
                {
                    if ( started == false )
                    {
                        game.LoadGraphicsContent();
                        started = true;
                    }

                    game.Process( main.ProcessEventArgs );


                    game.Render();


                },
                delegate
                {
                    game.Tick( main._tickEventArgs );
                } );

        }
    }
}
