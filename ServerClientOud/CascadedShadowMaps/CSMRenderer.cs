using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SampleCommon;
using MHGameWork.TheWizards.ServerClient.TWXNAEngine;

namespace MHGameWork.TheWizards.ServerClient.CascadedShadowMaps
{
    public enum ShadowFilteringType
    {
        PCF2x2 = 0,
        PCF3x3 = 1,
        PCF5x5 = 2,
        PCF7x7 = 3
    }

    public class CSMRenderer
    {

        public bool RenderDebug = false;

        public const int ShadowMapSize = 2048;
        public const int NumSplits = 4;

        DepthStencilBuffer dsBuffer;
        DepthStencilBuffer oldDS;
        public RenderTarget2D shadowMap;
        public BasicShader shadowMapShader;
        public Effect shadowMapEffect;
        RenderTarget2D shadowOcclusion;
        RenderTarget2D disabledShadowOcclusion;

        //ContentManager contentManager;
        IXNAGame game;
        GraphicsDevice graphicsDevice;

        FullScreenQuad fullScreenQuad;

        Vector3[] frustumCornersVS = new Vector3[ 8 ];
        Vector3[] frustumCornersWS = new Vector3[ 8 ];
        Vector3[] frustumCornersLS = new Vector3[ 8 ];
        Vector3[] farFrustumCornersVS = new Vector3[ 4 ];
        Vector3[] splitFrustumCornersVS = new Vector3[ 8 ];
        OrthographicCamera[] lightCameras = new OrthographicCamera[ NumSplits ];
        Matrix[] lightViewProjectionMatrices = new Matrix[ NumSplits ];
        Vector2[] lightClipPlanes = new Vector2[ NumSplits ];
        float[] splitDepths = new float[ NumSplits + 1 ];

        bool enabled = true;
        bool showCascadeSplits = false;
        ShadowFilteringType filteringType = ShadowFilteringType.PCF2x2;
        EffectTechnique[] shadowOcclusionTechniques = new EffectTechnique[ 4 ];


        public delegate void RenderPrimitives( IXNAGame game, BasicShader effect );

        /// <summary>
        /// Gets or sets a value indicating whether or not shadow occlusion
        /// should be calculated.
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public ShadowFilteringType ShadowFilteringType
        {
            get { return filteringType; }
            set { filteringType = value; }
        }

        public bool ShowCascadeSplits
        {
            get { return showCascadeSplits; }
            set { showCascadeSplits = value; }
        }

        /// <summary>
        /// Creates the renderer
        /// </summary>
        public CSMRenderer( IXNAGame _game )
        {
            game = _game;
            //this.contentManager = contentManager;
            this.graphicsDevice = _game.GraphicsDevice;

            // Load the effect we need
            shadowMapShader = BasicShader.LoadFromFXFile( game, new GameFile( System.Windows.Forms.Application.StartupPath + @"\Shaders\ShadowMap.fx" ) );
            shadowMapEffect = shadowMapShader.effect;//  contentManager.Load<Effect>( "Effects\\ShadowMap" );

            // Create the shadow map, using a 32-bit floating-point surface format
            shadowMap = new RenderTarget2D( graphicsDevice,
                                            ShadowMapSize * NumSplits,
                                            ShadowMapSize,
                                            1,
                                            SurfaceFormat.Single,
                                            RenderTargetUsage.DiscardContents );

            // Create a depth-stencil surface using the same dimensions as the shadow map
            dsBuffer = new DepthStencilBuffer( graphicsDevice,
                                                ShadowMapSize * NumSplits,
                                                ShadowMapSize,
                                                graphicsDevice.DepthStencilBuffer.Format );

            // Create the shadow occlusion texture using the same dimensions as the backbuffer
            shadowOcclusion = new RenderTarget2D( graphicsDevice,
                                                    graphicsDevice.PresentationParameters.BackBufferWidth,
                                                    graphicsDevice.PresentationParameters.BackBufferHeight,
                                                    1,
                                                    SurfaceFormat.Color,
                                                    RenderTargetUsage.DiscardContents );

            // Create a 1x1 texture that we'll clear to white and return when shadows are disabled
            disabledShadowOcclusion = new RenderTarget2D( graphicsDevice,
                                                            1,
                                                            1,
                                                            1,
                                                            SurfaceFormat.Color,
                                                            RenderTargetUsage.DiscardContents );

            // Create the full-screen quad
            fullScreenQuad = new FullScreenQuad( graphicsDevice );

            // We'll keep an array of EffectTechniques that will let us map a
            // ShadowFilteringType to a technique for calculating shadow occlusion
            shadowOcclusionTechniques[ 0 ] = shadowMapEffect.Techniques[ "CreateShadowTerm2x2PCF" ];
            shadowOcclusionTechniques[ 1 ] = shadowMapEffect.Techniques[ "CreateShadowTerm3x3PCF" ];
            shadowOcclusionTechniques[ 2 ] = shadowMapEffect.Techniques[ "CreateShadowTerm5x5PCF" ];
            shadowOcclusionTechniques[ 3 ] = shadowMapEffect.Techniques[ "CreateShadowTerm7x7PCF" ];
        }

        /// <summary>
        /// Renders a list of models to the shadow map, and returns a surface 
        /// containing the shadow occlusion factor
        /// </summary>
        /// <param name="modelList">The list of models to render</param>
        /// <param name="depthTexture">Texture containing depth for the scene</param>
        /// <param name="light">The light for which the shadow is being calculated</param>
        /// <param name="mainCamera">The camera viewing the scene containing the light</param>
        /// <returns>The shadow occlusion texture</returns>
        public RenderTarget2D Render( RenderPrimitives renderDelegate,
                                    RenderTarget2D depthTexture,
                                    DirectionalLight light,
                                    ICamera mainCamera )
        {
            if ( enabled )
            {
                // Set our targets
                oldDS = graphicsDevice.DepthStencilBuffer;
                graphicsDevice.DepthStencilBuffer = dsBuffer;
                graphicsDevice.SetRenderTarget( 0, shadowMap );
                graphicsDevice.Clear( ClearOptions.Target, Color.White, 1.0f, 0 );
                graphicsDevice.Clear( ClearOptions.DepthBuffer, Color.Black, 1.0f, 0 );

                // Get corners of the main camera's bounding frustum
                Matrix cameraTransform, viewMatrix;

                viewMatrix = mainCamera.View;
                cameraTransform = Matrix.Invert( viewMatrix );

                BoundingFrustum frustum = new BoundingFrustum( mainCamera.ViewProjection );

                frustum.GetCorners( frustumCornersWS );
                

                Vector3.Transform( frustumCornersWS, ref viewMatrix, frustumCornersVS );
                for ( int i = 0; i < 4; i++ )
                    farFrustumCornersVS[ i ] = frustumCornersVS[ i + 4 ];

                // Calculate the cascade splits.  We calculate these so that each successive
                // split is larger than the previous, giving the closest split the most amount
                // of shadow detail.  
                float N = NumSplits;
                float near = mainCamera.NearClip, far = mainCamera.FarClip;
                splitDepths[ 0 ] = near;
                splitDepths[ NumSplits ] = far;
                const float splitConstant = 0.95f;
                for ( int i = 1; i < splitDepths.Length - 1; i++ )
                    splitDepths[ i ] = splitConstant * near * (float)Math.Pow( far / near, i / N ) + ( 1.0f - splitConstant ) * ( ( near + ( i / N ) ) * ( far - near ) );

                // Render our scene geometry to each split of the cascade
                for ( int i = 0; i < NumSplits; i++ )
                {
                    float minZ = splitDepths[ i ];
                    float maxZ = splitDepths[ i + 1 ];

                    lightCameras[ i ] = CalculateFrustum( light, mainCamera, minZ, maxZ );

                    RenderShadowMap( renderDelegate, i );
                }

                RenderShadowOcclusion( mainCamera, depthTexture );

                graphicsDevice.DepthStencilBuffer = oldDS;

                return shadowOcclusion;
            }
            else
            {
                // If we're disabled, just clear our 1x1 texture to white and return it
                graphicsDevice.SetRenderTarget( 0, disabledShadowOcclusion );
                graphicsDevice.Clear( ClearOptions.Target, Color.White, 1.0f, 0 );

                return disabledShadowOcclusion;
            }
        }

        /// <summary>
        /// Determines the size of the frustum needed to cover the viewable area,
        /// then creates an appropriate orthographic projection.
        /// </summary>
        /// <param name="light">The directional light to use</param>
        /// <param name="mainCamera">The camera viewing the scene</param>
        /// <param name="minZ"></param>
        /// <param name="maxZ"></param>
        protected OrthographicCamera CalculateFrustum( DirectionalLight light, ICamera mainCamera, float minZ, float maxZ )
        {
            // Shorten the view frustum according to the shadow view distance
            Matrix cameraMatrix;
            cameraMatrix = mainCamera.ViewInverse;
            //mainCamera.GetWorldMatrix( out cameraMatrix );

            for ( int i = 0; i < 4; i++ )
                splitFrustumCornersVS[ i ] = frustumCornersVS[ i + 4 ] * ( minZ / mainCamera.FarClip );

            for ( int i = 4; i < 8; i++ )
                splitFrustumCornersVS[ i ] = frustumCornersVS[ i ] * ( maxZ / mainCamera.FarClip );

            Vector3.Transform( splitFrustumCornersVS, ref cameraMatrix, frustumCornersWS );

            // Find the centroid
            Vector3 frustumCentroid = new Vector3( 0, 0, 0 );
            for ( int i = 0; i < 8; i++ )
                frustumCentroid += frustumCornersWS[ i ];
            frustumCentroid *= ( 1 / 8.0F );

            // Position the shadow-caster camera so that it's looking at the centroid,
            // and backed up in the direction of the sunlight
            float distFromCentroid = MathHelper.Max( ( maxZ - minZ ), Vector3.Distance( splitFrustumCornersVS[ 4 ], splitFrustumCornersVS[ 5 ] ) ) + 50.0f;
            Matrix viewMatrix = Matrix.CreateLookAt( frustumCentroid - ( light.Direction * distFromCentroid ), frustumCentroid, new Vector3( 0, 1, 0 ) );

            // Determine the position of the frustum corners in light space
            Vector3.Transform( frustumCornersWS, ref viewMatrix, frustumCornersLS );

            // Calculate an orthographic projection by sizing a bounding box 
            // to the frustum coordinates in light space
            Vector3 mins = frustumCornersLS[ 0 ];
            Vector3 maxes = frustumCornersLS[ 0 ];
            for ( int i = 0; i < 8; i++ )
            {
                if ( frustumCornersLS[ i ].X > maxes.X )
                    maxes.X = frustumCornersLS[ i ].X;
                else if ( frustumCornersLS[ i ].X < mins.X )
                    mins.X = frustumCornersLS[ i ].X;
                if ( frustumCornersLS[ i ].Y > maxes.Y )
                    maxes.Y = frustumCornersLS[ i ].Y;
                else if ( frustumCornersLS[ i ].Y < mins.Y )
                    mins.Y = frustumCornersLS[ i ].Y;
                if ( frustumCornersLS[ i ].Z > maxes.Z )
                    maxes.Z = frustumCornersLS[ i ].Z;
                else if ( frustumCornersLS[ i ].Z < mins.Z )
                    mins.Z = frustumCornersLS[ i ].Z;
            }


            // Create an orthographic camera for use as a shadow caster
            const float nearClipOffset = 100.0f;
            OrthographicCamera lightCamera = new OrthographicCamera( mins.X, maxes.X, mins.Y, maxes.Y, -maxes.Z - nearClipOffset, -mins.Z );
            lightCamera.View = viewMatrix;


            if ( RenderDebug )
            {
                game.LineManager3D.AddCenteredBox( frustumCentroid - ( light.Direction*distFromCentroid ), 1.0f,
                                                   Color.Green );
                game.LineManager3D.AddLine( frustumCentroid - ( light.Direction*distFromCentroid ), frustumCentroid,
                                            Color.Yellow );
                game.LineManager3D.AddCenteredBox( frustumCentroid, 1.0f, Color.Red );
                game.LineManager3D.AddViewFrustum( new BoundingFrustum( mainCamera.ViewProjection ), Color.Purple );
                game.LineManager3D.AddViewFrustum( frustumCornersWS, Color.Red );
                Vector3[] temps = new Vector3[8];
                Matrix tm = Matrix.Invert( viewMatrix );
                BoundingBox bb = new BoundingBox( mins, maxes );
                Vector3[] temps2 = bb.GetCorners();
                Vector3.Transform( temps2, ref tm, temps );
                game.LineManager3D.AddViewFrustum( temps, Color.Green );
                game.LineManager3D.AddViewFrustum( new BoundingFrustum( lightCamera.ViewProjection ), Color.Purple );
            }

            return lightCamera;
        }


        /// <summary>
        /// Renders the shadow map using the orthographic camera created in
        /// CalculateFrustum.
        /// </summary>   
        protected void RenderShadowMap( RenderPrimitives renderDelegate, int splitIndex )
        {
            // Set the viewport for the current split            
            Viewport splitViewport = new Viewport();
            splitViewport.MinDepth = 0;
            splitViewport.MaxDepth = 1;
            splitViewport.Width = ShadowMapSize;
            splitViewport.Height = ShadowMapSize;
            splitViewport.X = splitIndex * ShadowMapSize;
            splitViewport.Y = 0;
            graphicsDevice.Viewport = splitViewport;

            // Set up the effect
            shadowMapEffect.CurrentTechnique = shadowMapEffect.Techniques[ "GenerateShadowMap" ]; //TODO: USE TECHNIQUE POINTER!
            shadowMapEffect.Parameters[ "g_matViewProj" ].SetValue( lightCameras[ splitIndex ].ViewProjection );

            shadowMapEffect.Begin();

            shadowMapEffect.CurrentTechnique.Passes[ 0 ].Begin();

            // Draw the scene
            renderDelegate( game, shadowMapShader );
            //foreach ( ModelInstance modelInstance in modelList )
            //    modelInstance.Draw( graphicsDevice, shadowMapEffect );

            shadowMapEffect.CurrentTechnique.Passes[ 0 ].End();

            shadowMapEffect.End();
        }

        /// <summary>
        /// Renders a texture containing the final shadow occlusion
        /// </summary>
        protected void RenderShadowOcclusion( ICamera mainCamera, RenderTarget2D depthTexture )
        {
            // Set the device to render to our shadow occlusion texture, and to use
            // the original DepthStencilSurface
            graphicsDevice.SetRenderTarget( 0, shadowOcclusion );
            graphicsDevice.DepthStencilBuffer = oldDS;

            Matrix cameraTransform;
            cameraTransform = mainCamera.ViewInverse;
            //mainCamera.GetWorldMatrix( out cameraTransform );

            // We'll use these clip planes to determine which split a pixel belongs to
            for ( int i = 0; i < NumSplits; i++ )
            {
                lightClipPlanes[ i ].X = -splitDepths[ i ];
                lightClipPlanes[ i ].Y = -splitDepths[ i + 1 ];

                lightViewProjectionMatrices[ i ] = lightCameras[i].ViewProjection;
                //lightCameras[ i ].GetViewProjMatrix( out  );
            }

            // Setup the Effect
            shadowMapEffect.CurrentTechnique = shadowOcclusionTechniques[ (int)filteringType ];
            shadowMapEffect.Parameters[ "g_matInvView" ].SetValue( cameraTransform );
            shadowMapEffect.Parameters[ "g_matLightViewProj" ].SetValue( lightViewProjectionMatrices );
            shadowMapEffect.Parameters[ "g_vFrustumCornersVS" ].SetValue( farFrustumCornersVS );
            shadowMapEffect.Parameters[ "g_vClipPlanes" ].SetValue( lightClipPlanes );
            shadowMapEffect.Parameters[ "ShadowMap" ].SetValue( shadowMap.GetTexture() );
            shadowMapEffect.Parameters[ "DepthTexture" ].SetValue( depthTexture.GetTexture() );
            shadowMapEffect.Parameters[ "g_vOcclusionTextureSize" ].SetValue( new Vector2( shadowOcclusion.Width, shadowOcclusion.Height ) );
            shadowMapEffect.Parameters[ "g_vShadowMapSize" ].SetValue( new Vector2( shadowMap.Width, shadowMap.Height ) );
            shadowMapEffect.Parameters[ "g_bShowSplitColors" ].SetValue( showCascadeSplits );

            // Begin effect
            shadowMapEffect.Begin( SaveStateMode.None );
            shadowMapEffect.CurrentTechnique.Passes[ 0 ].Begin();

            // Draw the full screen quad		
            fullScreenQuad.DrawOld( graphicsDevice );

            // End the effect
            shadowMapEffect.CurrentTechnique.Passes[ 0 ].End();
            shadowMapEffect.End();

            // Set to render to the back buffer
            graphicsDevice.SetRenderTarget( 0, null );
        }

    }
}
