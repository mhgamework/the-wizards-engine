using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Water
{
    public class WaterTest
    {
        XNAGame game;

        Water water;

        Camera camTemp = new Camera( Camera.FLY );
        SpectaterCamera specCam;
        bool inCamera;

        public WaterTest()
        {
            game = new XNAGame();
            game.InitializeEvent += new EventHandler( game_InitializeEvent );
            game.DrawEvent += new XNAGame.XNAGameLoopEventHandler( game_DrawEvent );
            game.UpdateEvent += new XNAGame.XNAGameLoopEventHandler( game_UpdateEvent );
            specCam = (SpectaterCamera)game.Camera;
            specCam.FarClip = 10000f;
            camTemp.SetLens( MathHelper.PiOver4, 4f / 3f, specCam.NearClip, specCam.FarClip );
            inCamera = true;
        }

        public void Run()
        {
            game.Run();
        }

        void game_UpdateEvent()
        {
            water.Update( game.Elapsed );
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.A ) ) inCamera = !inCamera;
            /*if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.NumPad2 ) )
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
            if ( game.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.LeftShift ) && game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.Right ) )
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
            }*/
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.R ) )
            {
                water.buildEffect( game, null );
            }
        }

        void game_DrawEvent()
        {

            if ( inCamera )
            {
                camTemp.LookAt( specCam.CameraPosition, specCam.CameraPosition - specCam.CameraDirection, Vector3.Up );
                camTemp.BuildView();
            }


            water.BuildMaps( camTemp, RenderWorld );


            water.Render( camTemp, Vector3.Normalize( new Vector3( -1, 0.5f, 0.5f ) ), Color.LightCyan.ToVector4() );
            //skyModel.Render( game, camTemp, new Plane() );
        }

        void game_InitializeEvent( object sender, EventArgs e )
        {
            Matrix mWaterWorld = Matrix.Identity;// r* t;

            /*--------------------------------------------------------------------------
             * Create the water and fill in the info structure
             *--------------------------------------------------------------------------*/
            WaterInfoTest info = new WaterInfoTest();
            info.fxFilename = System.Windows.Forms.Application.StartupPath + @"\Shaders\Water.fx";
            info.vertRows = 513;
            info.vertCols = 513;
            info.dx = 5.25f; //spaccing of grid lines - big distance
            info.dz = 5.25f;
            //info.dx = 0.5f; // small distance
            //info.dz = 0.5f;
            info.waveMapFileName0 = System.Windows.Forms.Application.StartupPath + @"\Content\wave0.dds";
            info.waveMapFileName1 = System.Windows.Forms.Application.StartupPath + @"\Content\wave1.dds";
            info.waveNMapVelocity0 = new Vector2( 0.03f, 0.05f );
            info.waveNMapVelocity1 = new Vector2( -0.01f, 0.03f );
            info.texScale = 24.0f;
            info.toWorld = mWaterWorld;

            TextureCube mEnvMap = TextureCube.FromFile( game.GraphicsDevice, System.Windows.Forms.Application.StartupPath + @"\Content\EnvMap.dds" );

            water = new Water( game, info, mEnvMap );

            //mWaterColor = new ColorValue( ( 255 * .13f ), ( 255 * .19f ), ( 255 * .22f ) );

            //use a sky model with a Hoffman-Preethem scattering method
            //mSkyModel = new SkyModel( 2000, mGDevice, typeof( Hoffman_Preethem ), mTerrain.Textures );

            //mGlarePostProcess = new GlarePostProcess( mGDevice, mGDevice.Viewport.Width, mGDevice.Viewport.Height );
            //mEnablePostProcess = true;

        }

        void RenderWorld( IXNAGame game )
        {
        }


        public static void RunTest()
        {
            WaterTest test = new WaterTest();
            test.Run();
        }
    }
}
