using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.Sky.Scattering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Sky
{
    public class SkyCSMRenderer : CascadedShadowMaps.ICSMRenderer
    {
        public SkyModel skyModel;

        Camera camTemp = new Camera( Camera.FLY );
        SpectaterCamera specCam;

        CascadedShadowMaps.CSMRendererService csmRendererService;

        float sunSpeed = 1;
        bool sunPause = true;


        public SkyCSMRenderer( TheWizards.Database.Database _database )
        {
            csmRendererService = _database.FindService<CascadedShadowMaps.CSMRendererService>();

        }

        #region IRenderer Members

        public void Render( XNAGame game )
        {
            camTemp.LookAt( specCam.CameraPosition, specCam.CameraPosition + specCam.CameraDirection, Vector3.Up );
            camTemp.BuildView();
            skyModel.Render( game, camTemp, new Plane() );
        }

        public void Update( XNAGame game )
        {
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.NumPad2 ) )
            {

                sunSpeed *= 1.1f;


            }
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.NumPad1 ) )
            {
                if ( Math.Abs( sunSpeed ) > 0.1f )
                {
                    sunSpeed *= 0.9f;
                }

            }
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.NumPad3 ) )
            {
                sunSpeed = -sunSpeed;
            }
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.NumPad0 ) )
            {
                sunPause = !sunPause;
            }

            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.NumPad5 ) )
            {
                skyModel.ScatterMethod.Rayleigh = 3.8f;
                skyModel.ScatterMethod.Mie = 0.0001f;
                skyModel.ScatterMethod.Inscattering = 0.91f;
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
            }
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.R ) )
            {
                Initialize( game );
            }

            if ( !sunPause )
            {
                skyModel.SunPosition += sunSpeed*game.Elapsed;
                if ( skyModel.SunPosition == 90 ) skyModel.SunPosition = -90;
                if ( skyModel.SunPosition == 90 ) skyModel.SunPosition = 90;

            }

            if ( csmRendererService != null )
            {
                csmRendererService.light.Direction = -skyModel.SunDirection;
                csmRendererService.light.Color = new Vector3( skyModel.ScatterMethod.SunColorAndIntensity.X
                                                              , skyModel.ScatterMethod.SunColorAndIntensity.Y
                                                              , skyModel.ScatterMethod.SunColorAndIntensity.Z );
                //skyModel.SunDirection = 
            }
        }

        public void Initialize( XNAGame game )
        {
            specCam = (SpectaterCamera)game.Camera;
            Texture2D[] textures = new Texture2D[ 0 ];
            //use a sky model with a Hoffman-Preethem scattering method
            //SkyModel = new SkyModel(game, 2000, typeof( Hoffman_Preethem ), mTerrain.Textures );
            skyModel = new SkyModel( game, 2000, typeof( Hoffman_Preethem ), textures );
        }

        #endregion

        #region ICSMRenderer Members


        public void RenderDepth( XNAGame game, BasicShader depthShader )
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        public void RenderNormal( XNAGame game, BasicShader normalShader )
        {
            Render( game );
            //throw new Exception( "The method or operation is not implemented." );
        }

        public void RenderShadowMap( XNAGame game, BasicShader shadowMapShader )
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        #endregion
    }
}
