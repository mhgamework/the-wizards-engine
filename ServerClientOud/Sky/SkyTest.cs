using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.Sky.Scattering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Sky
{
    public class SkyTest
    {
        XNAGame game;

        SkyModel skyModel;

        // NOTE: projection matrix not initialized?
        Camera camTemp = new Camera( Camera.FLY );
        SpectaterCamera specCam;

        public SkyTest()
        {
            game = new XNAGame();
            game.InitializeEvent += new EventHandler( game_InitializeEvent );
            game.DrawEvent += new XNAGame.XNAGameLoopEventHandler( game_DrawEvent );
            game.UpdateEvent += new XNAGame.XNAGameLoopEventHandler( game_UpdateEvent );
            specCam = (SpectaterCamera)game.Camera;

            
        }

        public void Run()
        {
            game.Run();
        }

        void game_UpdateEvent()
        {
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.NumPad2 ) )
            {
                skyModel.SunPosition += 0.8f;
            }
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.NumPad1 ) )
            {
                skyModel.SunPosition -= 0.8f;
            }

            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.Up ) )
            {
                skyModel.ScatterMethod.Rayleigh += 1.2f;
            }
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.Down ) )
            {
                skyModel.ScatterMethod.Rayleigh -= 1.2f;
            }
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.Left ) )
            {
                skyModel.ScatterMethod.Mie += .0001f;
            }
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.Right ) )
            {
                skyModel.ScatterMethod.Mie -= .0001f;
            }
            if ( game.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) && game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.Right ) )
            {
                skyModel.ScatterMethod.Mie -= .00001f;

            }
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.M ) )
            {
                skyModel.ScatterMethod.Inscattering += .009f;
            }
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.N ) )
            {
                skyModel.ScatterMethod.Inscattering -= .009f;
            }
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.R ) )
            {
                game_InitializeEvent( null, null );
            }
        }

        void game_DrawEvent()
        {
            

            camTemp.LookAt( specCam.CameraPosition, specCam.CameraPosition + specCam.CameraDirection, Vector3.Up );
            camTemp.BuildView();
            skyModel.Render( game, camTemp, new Plane() );
        }

        void game_InitializeEvent( object sender, EventArgs e )
        {
            Texture2D[] textures = new Texture2D[ 0 ];
            //use a sky model with a Hoffman-Preethem scattering method
            //SkyModel = new SkyModel(game, 2000, typeof( Hoffman_Preethem ), mTerrain.Textures );
            skyModel = new SkyModel( game, 2000, typeof( Hoffman_Preethem ), textures );


        }

        public static void RunTest()
        {
            SkyTest test = new SkyTest();
            test.Run();
        }
    }
}
