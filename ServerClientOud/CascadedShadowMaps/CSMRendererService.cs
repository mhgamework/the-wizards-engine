using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.CascadedShadowMaps
{
    public class CSMRendererService : Database.IGameService, TWClient.IRenderer
    {

        private XNAGame game;
        private TWModel wall;
        private CSMRenderer csmRenderer;

        

        // A texture for storing scene depth information
        RenderTarget2D depthTexture;

        // The Effect's we'll use
        BasicShader depthShader;
        Effect depthEffect;

        BasicShader modelShader;
        Effect modelEffect;

        public List<ICSMRenderer> CSMRenderers = new List<ICSMRenderer>();
        private bool renderDebug = false;

        public bool RenderDebug
        {
            get { return renderDebug; }
            set { renderDebug = value;if (csmRenderer != null) csmRenderer.RenderDebug = value; }
        }

        SpectaterCamera freeCamera;
        SpectaterCamera shadowCamera;

        /// <summary>
        /// Temp
        /// </summary>
        public Texture2D shadowOcclusionTexture;
        public Texture2D GetShadowOcclusionTexture()
        {
            return shadowOcclusionTexture;
        }

        /*SpectaterCamera freeCamera;
        SpectaterCamera shadowCamera;*/

        public SampleCommon.DirectionalLight light = new SampleCommon.DirectionalLight();

        public CSMRendererService( XNAGame _game, TheWizards.Database.Database _database )
        {
            game = _game;
            //NOTE: this was removed, this is app logic not csmrenderer logic.
            //_database.FindService<TWClient.RendererService>().AddIRenderer( this );

            freeCamera = (SpectaterCamera)game.Camera;
            //game.SetCamera( new SpectaterCamera( game, 1, 400f ) );
            shadowCamera = new SpectaterCamera( game, 1.0f, 400.0f );
            light.Direction = new Vector3( 1, 1, 1 );
            light.Direction.Normalize();
            light.Color = new Vector3( 1, 1, 1 );
        }



        public void Render()
        {
            csmRenderer.ShadowFilteringType = ShadowFilteringType.PCF7x7;
            game.GraphicsDevice.RenderState.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None;


            game.SetCamera( shadowCamera );

            DrawDepth();

            RenderTarget2D shadowOcclusionMap = csmRenderer.Render( RenderShadowMap, depthTexture, light, game.Camera );

            Texture2D tex = shadowOcclusionMap.GetTexture();
            shadowOcclusionTexture = tex;
            /*if ( game.Camera == freeCamera )
            {
                DrawMainLightingPass( null );
            }
            else
            {*/
            if ( freeCamera.Enabled ) game.SetCamera( freeCamera );
            DrawMainLightingPass( shadowOcclusionMap.GetTexture() );
            /* }*/

            

            if ( RenderDebug )
            {

                game.SpriteBatch.Begin();
                game.SpriteBatch.Draw( depthTexture.GetTexture(), new Rectangle( 10, 10, 150, 150 ), Color.White );
                game.SpriteBatch.Draw( csmRenderer.shadowMap.GetTexture(), new Rectangle( 170, 10, 150*4, 150 ),
                                       Color.White );
                game.SpriteBatch.Draw( shadowOcclusionMap.GetTexture(), new Rectangle( 10, 310, 300, 300 ), Color.White );
                game.SpriteBatch.End();
            }


            //tex.Dispose();

            //wall.Render();

        }

        /// <summary>
        /// Draws the main pass for the scene 
        /// </summary>
        /// <param name="shadowOcclusion">The shadow occlusion texture</param>
        private void DrawMainLightingPass( Texture2D shadowOcclusion )
        {


            GraphicsDevice GraphicsDevice = game.GraphicsDevice;
            // Set to render to the back buffer
            GraphicsDevice.SetRenderTarget( 0, null );

            GraphicsDevice.RenderState.DepthBufferEnable = true;
            GraphicsDevice.RenderState.DepthBufferWriteEnable = true;

            // Clear the background
            GraphicsDevice.Clear( ClearOptions.Target, Color.CornflowerBlue, 1.0f, 0 );
            GraphicsDevice.Clear( ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0 );

            /*for ( int i = 0; i < CSMRenderers.Count; i++ )
            {
                CSMRenderers[ i ].RenderNormal( game, modelShader );
            }

            return;*/

            // Setup the Effect
            int width = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int height = GraphicsDevice.PresentationParameters.BackBufferHeight;
            modelEffect.CurrentTechnique = modelEffect.Techniques[ "Model" ];
            modelEffect.Parameters[ "g_matViewProj" ].SetValue( game.Camera.ViewProjection );
            modelEffect.Parameters[ "g_vCameraPositionWS" ].SetValue( game.Camera.ViewInverse.Translation );
            modelEffect.Parameters[ "g_vRTDimensions" ].SetValue( new Vector2( width, height ) );

            // Normally these would come from Model data, but for this sample we'll just use set values
            modelEffect.Parameters[ "g_vDiffuseAlbedo" ].SetValue( new Vector3( 1.0f ) );
            modelEffect.Parameters[ "g_vSpecularAlbedo" ].SetValue( new Vector3( 1.0f ) );
            modelEffect.Parameters[ "g_fSpecularPower" ].SetValue( 32.0f );

            // Lighting parameters
            modelEffect.Parameters[ "g_vLightDirectionWS" ].SetValue( light.Direction );
            modelEffect.Parameters[ "g_vLightColor" ].SetValue( light.Color );
            modelEffect.Parameters[ "g_vAmbientColor" ].SetValue( new Vector3( 0.2f ) );

            // Bind the shadow occlusion texture
            modelEffect.Parameters[ "ShadowOcclusion" ].SetValue( shadowOcclusion );

            // Begin the Effect
            modelEffect.Begin();

            modelEffect.CurrentTechnique.Passes[ 0 ].Begin();

            // Draw the models
            for ( int i = 0; i < CSMRenderers.Count; i++ )
            {
                CSMRenderers[ i ].RenderNormal( game, modelShader );
            }
            //RenderPrimitives( game, modelEffect );
            //foreach ( ModelInstance modelInstance in modelList )
            //    modelInstance.Draw( GraphicsDevice, modelEffect );

            // End the Effect
            modelEffect.CurrentTechnique.Passes[ 0 ].End();

            modelEffect.End();
        }

        /// <summary>
        /// Draws depth for the scene to a texture
        /// </summary>
        private void DrawDepth()
        {
            GraphicsDevice GraphicsDevice = game.GraphicsDevice;

            // Set to render to our depth texture
            GraphicsDevice.SetRenderTarget( 0, depthTexture );

            // Clear the texture
            GraphicsDevice.Clear( ClearOptions.Target, new Vector4( 1.0f ), 1.0f, 0 );
            GraphicsDevice.Clear( ClearOptions.DepthBuffer, new Vector4( 1.0f ), 1.0f, 0 );

            // Setup our depth Effect
            depthEffect.CurrentTechnique = depthEffect.Techniques[ "LinearDepth" ];
            depthEffect.Parameters[ "g_matView" ].SetValue( game.Camera.View );
            depthEffect.Parameters[ "g_matProj" ].SetValue( game.Camera.Projection );
            depthEffect.Parameters[ "g_fFarClip" ].SetValue( game.Camera.FarClip );

            // Begin the Effect
            depthEffect.Begin();

            depthEffect.CurrentTechnique.Passes[ 0 ].Begin();

            // Draw the models
            for ( int i = 0; i < CSMRenderers.Count; i++ )
            {
                CSMRenderers[ i ].RenderDepth( game, depthShader );
            }
            //RenderPrimitives( game, depthEffect );
            //foreach ( ModelInstance modelInstance in modelList )
            //    modelInstance.Draw( GraphicsDevice, depthEffect );

            // End the Effect
            depthEffect.CurrentTechnique.Passes[ 0 ].End();

            depthEffect.End();
        }

        void RenderShadowMap( IXNAGame _game, BasicShader shadowMapShader )
        {

            for ( int i = 0; i < CSMRenderers.Count; i++ )
            {
                CSMRenderers[ i ].RenderShadowMap( game, shadowMapShader );
            }


        }

        public void Initialize()
        {

            for ( int i = 0; i < CSMRenderers.Count; i++ )
            {
                CSMRenderers[ i ].Initialize( game );
            }



            // Make the texture where we'll store depth.  We'll use a 
            // 32-bit floating-point format for precision.
            int width = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int height = game.GraphicsDevice.PresentationParameters.BackBufferHeight;
            depthTexture = new RenderTarget2D( game.GraphicsDevice, width, height, 1, SurfaceFormat.Single, RenderTargetUsage.DiscardContents );

            csmRenderer = new CSMRenderer( game );

            depthShader = BasicShader.LoadFromFXFile( game, new GameFile( System.Windows.Forms.Application.StartupPath + @"\Shaders\Depth.fx" ) );
            depthEffect = depthShader.effect;//  contentManager.Load<Effect>( "Effects\\ShadowMap" );
            modelShader = BasicShader.LoadFromFXFile( game, new GameFile( System.Windows.Forms.Application.StartupPath + @"\Shaders\Model.fx" ) );
            modelEffect = modelShader.effect;//  contentManager.Load<Effect>( "Effects\\ShadowMap" );

            //throw new Exception( "The method or operation is not implemented." );
        }


        #region IDisposable Members

        public void Dispose()
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        #endregion

        #region IRenderer Members

        public void Render( XNAGame game )
        {
            Render();
        }

        public void Update( XNAGame game )
        {
            //csmRenderer.ShowCascadeSplits = true;
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.R ) )
            {
                depthShader.effect.Dispose();
                modelShader.effect.Dispose();
                depthShader = BasicShader.LoadFromFXFile( game, new GameFile( System.Windows.Forms.Application.StartupPath + @"\Shaders\Depth.fx" ) );
                depthEffect = depthShader.effect;//  contentManager.Load<Effect>( "Effects\\ShadowMap" );
                modelShader = BasicShader.LoadFromFXFile( game, new GameFile( System.Windows.Forms.Application.StartupPath + @"\Shaders\Model.fx" ) );
                modelEffect = modelShader.effect;//  contentManager.Load<Effect>( "Effects\\ShadowMap" );
                csmRenderer.shadowMapShader = BasicShader.LoadFromFXFile( game, new GameFile( System.Windows.Forms.Application.StartupPath + @"\Shaders\ShadowMap.fx" ) );
                csmRenderer.shadowMapEffect = csmRenderer.shadowMapShader.effect;//  contentManager.Load<Effect>( "Effects\\ShadowMap" );
            }
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.L ) )
            {
                light.Direction = ( (SpectaterCamera)game.Camera ).CameraDirection;
            }
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.K ) )
            {
                csmRenderer.ShowCascadeSplits = !csmRenderer.ShowCascadeSplits;
            }
            /*{
                SpectaterCamera cam = new SpectaterCamera( game, 1, 100 );
                cam.CameraDirection = ( (SpectaterCamera)game.Camera ).CameraDirection;
                cam.CameraPosition = ( (SpectaterCamera)game.Camera ).CameraPosition;
                cam.UpdateCameraInfo();
                mainCamera.ViewMatrix = cam.View;
                mainCamera.ViewProjectionMatrix = cam.ViewProjection;
            }
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.J ) )
            {
                SpectaterCamera cam = new SpectaterCamera( game, 1, 100 );
                cam.CameraDirection = new Vector3( 0.2802523f, -0.3189588f, 0.9053861f );
                cam.CameraPosition = new Vector3( -10.48257f, 108.4995f, -101.7325f );
                cam.UpdateCameraInfo();
                mainCamera.ViewMatrix = cam.View;
                mainCamera.ViewProjectionMatrix = cam.ViewProjection;

                light.Direction = new Vector3( -0.4796122f, -0.1460826f, 0.8652353f );
            }*/
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.A ) )
            {
                if ( game.Camera == freeCamera )
                {

                    game.SetCamera( shadowCamera );

                    freeCamera.Enabled = false;
                    shadowCamera.Enabled = true;


                }
                else
                {
                    game.SetCamera( freeCamera );
                    shadowCamera.Enabled = false;
                    freeCamera.Enabled = true;
                }

                /*SpectaterCamera cam = new SpectaterCamera( game, 1.0f, 400.0f );
                cam.CameraDirection = ( (SpectaterCamera)game.Camera ).CameraDirection;
                cam.CameraPosition = ( (SpectaterCamera)game.Camera ).CameraPosition;
                cam.UpdateCameraInfo();
                //mainCamera = new SampleCommon.FirstPersonCamera( MathHelper.PiOver4, 4 / 3f, 1.23456f, 10000.0f );
                mainCamera.ViewMatrix = cam.View;
                mainCamera.ViewProjectionMatrix = cam.ViewProjection;*/




            }
            for ( int i = 0; i < CSMRenderers.Count; i++ )
            {
                CSMRenderers[ i ].Update( game );
            }
        }

        public void Initialize( XNAGame game )
        {
            Initialize();
        }

        #endregion
    }
}
