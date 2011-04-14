using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.CascadedShadowMaps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Water
{
    public class WaterCSMRenderer : TWClient.IRenderer
    {
        private CSMRendererService csmRendererService;
        private CSMRenderer csmRenderer;

        // A texture for storing scene depth information
        RenderTarget2D depthTexture;

        // The Effect's we'll use
        BasicShader depthShader;
        Effect depthEffect;


        public List<ICSMRenderer> CSMRenderers = new List<ICSMRenderer>();
        private bool renderDebug = false;

        public bool RenderDebug
        {
            get { return renderDebug; }
            set { renderDebug = value; csmRenderer.RenderDebug = value; }
        }

        Sky.SkyCSMRenderer skyCSMRenderer;

        Texture2D shadowOcclusionTexture;
        public Texture2D GetShadowOcclusionTexture()
        {
            return shadowOcclusionTexture;
        }

        /*SpectaterCamera freeCamera;
        SpectaterCamera shadowCamera;*/

        public SampleCommon.DirectionalLight light = new SampleCommon.DirectionalLight();


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
            /*modelEffect.CurrentTechnique = modelEffect.Techniques[ "Model" ];
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

            modelEffect.CurrentTechnique.Passes[ 0 ].Begin();*/

            // Draw the models
            for ( int i = 0; i < CSMRenderers.Count; i++ )
            {
                CSMRenderers[ i ].RenderNormal( (XNAGame)game, null );
            }
            water.Render( camTemp, -light.Direction, new Vector4( light.Color, 1 ) );

            //RenderPrimitives( game, modelEffect );
            //foreach ( ModelInstance modelInstance in modelList )
            //    modelInstance.Draw( GraphicsDevice, modelEffect );

            // End the Effect
            /*modelEffect.CurrentTechnique.Passes[ 0 ].End();

            modelEffect.End();*/
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




        public void Dispose()
        {
            //throw new Exception( "The method or operation is not implemented." );
        }























        XNAGame game;
        //CascadedShadowMaps.CSMRendererService csmRendererService;
        Water water;

        Camera camTemp = new Camera( Camera.FLY );
        SpectaterCamera specCam;

        public WaterCSMRenderer( XNAGame _game, TheWizards.Database.Database _database, Sky.SkyCSMRenderer _skyCSMRenderer )
        {
            game = _game;
            skyCSMRenderer = _skyCSMRenderer;
            csmRendererService = new CSMRendererService( game, _database );
            _database.AddService( csmRendererService );
            //csmRendererService = _database.FindService<CascadedShadowMaps.CSMRendererService>();

            specCam = (SpectaterCamera)game.Camera;
            //specCam.FarClip = 10000f;
            camTemp.SetLens( MathHelper.PiOver4, 4f / 3f, specCam.NearClip, specCam.FarClip );
        }


        public void Render( XNAGame _game )
        {

            light.Direction = -skyCSMRenderer.skyModel.SunDirection;
            light.Color = new Vector3( skyCSMRenderer.skyModel.ScatterMethod.SunColorAndIntensity.X
                                                          , skyCSMRenderer.skyModel.ScatterMethod.SunColorAndIntensity.Y
                                                          , skyCSMRenderer.skyModel.ScatterMethod.SunColorAndIntensity.Z );
            csmRendererService.light = light;
            camTemp.LookAt( specCam.CameraPosition, specCam.CameraPosition - specCam.CameraDirection, Vector3.Up );
            camTemp.BuildView();

            csmRenderer.ShadowFilteringType = ShadowFilteringType.PCF7x7;
            game.GraphicsDevice.RenderState.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None;


            //game.SetCamera( shadowCamera );

            DrawDepth();


            RenderTarget2D shadowOcclusionMap = csmRenderer.Render( RenderShadowMap, depthTexture, light, game.Camera );



            Texture2D tex = shadowOcclusionMap.GetTexture();
            shadowOcclusionTexture = tex;
            csmRendererService.shadowOcclusionTexture = tex;

            //water.BuildMaps( camTemp, RenderWorld );
            water.BuildRefractMap( camTemp, RenderWorldRefract );
            water.BuildReflectMap( camTemp, RenderWorld );
            water.mEffect.effect.Parameters[ "gDepthMap" ].SetValue( depthTexture.GetTexture() );
            int width = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int height = game.GraphicsDevice.PresentationParameters.BackBufferHeight;
            water.mEffect.effect.Parameters[ "BackbufferSize" ].SetValue( new Vector2( width, height ) );


            DrawMainLightingPass( shadowOcclusionMap.GetTexture() );


            if ( RenderDebug )
            {

                _game.SpriteBatch.Begin();
                _game.SpriteBatch.Draw( water.mReflectMap.GetTexture(), new Rectangle( 10, 10, 300, 300 ), Color.White );
                _game.SpriteBatch.Draw( water.mRefractMap.GetTexture(), new Rectangle( 470, 10, 300, 300 ), Color.White );
                _game.SpriteBatch.End();


                /*game.SpriteBatch.Begin();
                game.SpriteBatch.Draw( depthTexture.GetTexture(), new Rectangle( 10, 10, 150, 150 ), Color.White );
                game.SpriteBatch.Draw( csmRenderer.shadowMap.GetTexture(), new Rectangle( 170, 10, 150 * 4, 150 ),
                                       Color.White );
                game.SpriteBatch.Draw( shadowOcclusionMap.GetTexture(), new Rectangle( 10, 310, 300, 300 ), Color.White );
                game.SpriteBatch.End();*/
            }





            return;















            /*

            //if ( inCamera )
            {
                camTemp.LookAt( specCam.CameraPosition, specCam.CameraPosition - specCam.CameraDirection, Vector3.Up );
                camTemp.BuildView();
            }


            water.BuildMaps( camTemp, RenderWorld );



            RenderWorld( game );

            water.Render( camTemp, Vector3.Normalize( new Vector3( -1, 0.5f, 0.5f ) ), Color.LightCyan.ToVector4() );

            if ( RenderDebug )
            {

                _game.SpriteBatch.Begin();
                //_game.SpriteBatch.Draw( water.mReflectMap.GetTexture(), new Rectangle( 10, 10, 150, 150 ), Color.White );
                //_game.SpriteBatch.Draw( water.mRefractMap.GetTexture(), new Rectangle( 170, 10, 150, 150 ), Color.White );
                _game.SpriteBatch.End();


                game.SpriteBatch.Begin();
                game.SpriteBatch.Draw( depthTexture.GetTexture(), new Rectangle( 10, 10, 150, 150 ), Color.White );
                game.SpriteBatch.Draw( csmRenderer.shadowMap.GetTexture(), new Rectangle( 170, 10, 150 * 4, 150 ),
                                       Color.White );
                game.SpriteBatch.Draw( shadowOcclusionMap.GetTexture(), new Rectangle( 10, 310, 300, 300 ), Color.White );
                game.SpriteBatch.End();
            }
            //skyModel.Render( game, camTemp, new Plane() );*/
        }

        private void RenderWorld( IXNAGame _game )
        {
            for ( int i = 0; i < CSMRenderers.Count; i++ )
            {
                CSMRenderers[ i ].RenderNormal( (XNAGame)game, null );
            }
            //csmRendererService.Render();
        }
        private void RenderWorldRefract( IXNAGame _game )
        {
            for ( int i = 0; i < CSMRenderers.Count; i++ )
            {
                if ( CSMRenderers[ i ] != skyCSMRenderer )
                    CSMRenderers[ i ].RenderNormal( (XNAGame)game, null );
            }
            //csmRendererService.Render();
        }

        public void Update( XNAGame game )
        {
            //csmRendererService.Update( (XNAGame) game );
            water.Update( game.Elapsed );

            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.R ) )
            {
                water.buildEffect( game, null );
            }
            if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.V ) )
            {
                renderDebug = !renderDebug;
            }
            /*if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.A ) )
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



            }*/
            for ( int i = 0; i < CSMRenderers.Count; i++ )
            {
                CSMRenderers[ i ].Update( game );
            }
        }

        public void Initialize( XNAGame _game )
        {
            //csmRendererService.Initialize( (XNAGame)game );



            Matrix mWaterWorld = Matrix.Identity;// r* t;
            //mWaterWorld = Matrix.CreateTranslation( new Vector3( 0, 20, 0 ) );

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
            //modelShader = BasicShader.LoadFromFXFile( game, new GameFile( System.Windows.Forms.Application.StartupPath + @"\Shaders\Model.fx" ) );
            //modelEffect = modelShader.effect;//  contentManager.Load<Effect>( "Effects\\ShadowMap" );

            //throw new Exception( "The method or operation is not implemented." );
        }

    }
}
