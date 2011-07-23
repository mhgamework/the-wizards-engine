using System;
using System.IO;
using DirectX11.Graphics;
using SlimDX;
using SlimDX.Direct3D11;

namespace DirectX11.Rendering.CSM
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
        private readonly DX11Game game;

        public bool RenderDebug = false;

        public const int ShadowMapSize = 2048;
        public const int NumSplits = 4;

        public Texture2D shadowMap;
        public BasicShader shadowMapShader;
        public Effect shadowMapEffect;

        //ContentManager contentManager;

        FullScreenQuad fullScreenQuad;

        Vector3[] frustumCornersVS = new Vector3[8];
        Vector3[] frustumCornersWS = new Vector3[8];
        Vector3[] frustumCornersLS = new Vector3[8];
        Vector3[] farFrustumCornersVS = new Vector3[4];
        Vector3[] splitFrustumCornersVS = new Vector3[8];
        OrthographicCamera[] lightCameras = new OrthographicCamera[NumSplits];
        Matrix[] lightViewProjectionMatrices = new Matrix[NumSplits];
        Vector2[] lightClipPlanes = new Vector2[NumSplits];
        float[] splitDepths = new float[NumSplits + 1];

        bool enabled = true;
        bool showCascadeSplits = false;
        ShadowFilteringType filteringType = ShadowFilteringType.PCF2x2;
        string[] shadowOcclusionTechniques = new string[4];
        private DeviceContext context;
        private DepthStencilView shadowMapDsv;
        private ShaderResourceView shadowMapRV;


        public delegate void RenderPrimitives(Matrix viewProjection);

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
        public CSMRenderer(DX11Game game)
        {
            this.game = game;
            var device = game.Device;
            context = device.ImmediateContext;

            // Load the effect we need
            shadowMapShader = BasicShader.LoadAutoreload(game, new FileInfo(@"..\..\DirectX11\Shaders\CSM\CSM.fx"));

            // Create the shadow map, using a 32-bit floating-point surface format
            shadowMap = new Texture2D(device, new Texture2DDescription
                                                  {
                                                      Width = ShadowMapSize * NumSplits,
                                                      Height = ShadowMapSize,
                                                      MipLevels = 1,
                                                      ArraySize = 1,
                                                      BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
                                                      Format = SlimDX.DXGI.Format.R32_Typeless

                                                  });

            shadowMapDsv = new DepthStencilView(device, shadowMap, new DepthStencilViewDescription
                                                                       {
                                                                           ArraySize = 1,
                                                                           Dimension = DepthStencilViewDimension.Texture2D,
                                                                           Flags = DepthStencilViewFlags.None,
                                                                           Format = SlimDX.DXGI.Format.D32_Float,
                                                                           MipSlice = 0
                                                                       });
            shadowMapRV = new ShaderResourceView(device, shadowMap, new ShaderResourceViewDescription
                                                                {
                                                                    ArraySize = 1,
                                                                    Dimension = ShaderResourceViewDimension.Texture2D,
                                                                    MipLevels = 1,
                                                                    MostDetailedMip = 0,
                                                                    Format = SlimDX.DXGI.Format.R32_Float
                                                                });


            // Create the full-screen quad
            fullScreenQuad = new FullScreenQuad(device);

            // We'll keep an array of EffectTechniques that will let us map a
            // ShadowFilteringType to a technique for calculating shadow occlusion
            shadowOcclusionTechniques[0] = "CreateShadowTerm2x2PCF";
            shadowOcclusionTechniques[1] = "CreateShadowTerm3x3PCF";
            shadowOcclusionTechniques[2] = "CreateShadowTerm5x5PCF";
            shadowOcclusionTechniques[3] = "CreateShadowTerm7x7PCF";
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
        public void UpdateShadowMap(RenderPrimitives renderDelegate, DirectionalLight light, ICamera mainCamera)
        {
            context.ClearDepthStencilView(shadowMapDsv, DepthStencilClearFlags.Depth, 1, 0);
            context.ClearState();
            context.OutputMerger.SetTargets(shadowMapDsv);



            // Get corners of the main camera's bounding frustum
            Matrix cameraTransform, viewMatrix;

            viewMatrix = mainCamera.View;
            cameraTransform = Matrix.Invert(viewMatrix);



            BoundingFrustum frustum = new BoundingFrustum(mainCamera.ViewProjection);

            frustum.GetCorners(frustumCornersWS);


            Vector3.TransformCoordinate(frustumCornersWS, ref viewMatrix, frustumCornersVS);
            for (int i = 0; i < 4; i++)
                farFrustumCornersVS[i] = frustumCornersVS[i + 4];

            // Calculate the cascade splits.  We calculate these so that each successive
            // split is larger than the previous, giving the closest split the most amount
            // of shadow detail.  
            float N = NumSplits;
            float near = mainCamera.NearClip, far = mainCamera.FarClip;
            splitDepths[0] = near;
            splitDepths[NumSplits] = far;
            const float splitConstant = 0.95f;
            for (int i = 1; i < splitDepths.Length - 1; i++)
                splitDepths[i] = splitConstant * near * (float)Math.Pow(far / near, i / N) + (1.0f - splitConstant) * ((near + (i / N)) * (far - near));

            // Render our scene geometry to each split of the cascade
            for (int i = 0; i < NumSplits; i++)
            {
                float minZ = splitDepths[i];
                float maxZ = splitDepths[i + 1];

                lightCameras[i] = CalculateFrustum(light, mainCamera, minZ, maxZ);

                renderShadowMap(renderDelegate, i);
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
        protected OrthographicCamera CalculateFrustum(DirectionalLight light, ICamera mainCamera, float minZ, float maxZ)
        {
            // Shorten the view frustum according to the shadow view distance
            Matrix cameraMatrix;
            cameraMatrix = mainCamera.ViewInverse;
            //mainCamera.GetWorldMatrix( out cameraMatrix );

            for (int i = 0; i < 4; i++)
                splitFrustumCornersVS[i] = frustumCornersVS[i + 4] * (minZ / mainCamera.FarClip);

            for (int i = 4; i < 8; i++)
                splitFrustumCornersVS[i] = frustumCornersVS[i] * (maxZ / mainCamera.FarClip);

            Vector3.TransformCoordinate(splitFrustumCornersVS, ref cameraMatrix, frustumCornersWS);

            // Find the centroid
            Vector3 frustumCentroid = new Vector3(0, 0, 0);
            for (int i = 0; i < 8; i++)
                frustumCentroid += frustumCornersWS[i];
            frustumCentroid *= (1 / 8.0F);

            // Position the shadow-caster camera so that it's looking at the centroid,
            // and backed up in the direction of the sunlight
            float distFromCentroid = MathHelper.Max((maxZ - minZ), Vector3.Distance(splitFrustumCornersVS[4], splitFrustumCornersVS[5])) + 50.0f;
            Matrix viewMatrix = Matrix.LookAtRH(frustumCentroid - (light.Direction * distFromCentroid), frustumCentroid, new Vector3(0, 1, 0));

            // Determine the position of the frustum corners in light space
            Vector3.TransformCoordinate(frustumCornersWS, ref viewMatrix, frustumCornersLS);

            // Calculate an orthographic projection by sizing a bounding box 
            // to the frustum coordinates in light space
            Vector3 mins = frustumCornersLS[0];
            Vector3 maxes = frustumCornersLS[0];
            for (int i = 0; i < 8; i++)
            {
                if (frustumCornersLS[i].X > maxes.X)
                    maxes.X = frustumCornersLS[i].X;
                else if (frustumCornersLS[i].X < mins.X)
                    mins.X = frustumCornersLS[i].X;
                if (frustumCornersLS[i].Y > maxes.Y)
                    maxes.Y = frustumCornersLS[i].Y;
                else if (frustumCornersLS[i].Y < mins.Y)
                    mins.Y = frustumCornersLS[i].Y;
                if (frustumCornersLS[i].Z > maxes.Z)
                    maxes.Z = frustumCornersLS[i].Z;
                else if (frustumCornersLS[i].Z < mins.Z)
                    mins.Z = frustumCornersLS[i].Z;
            }


            // Create an orthographic camera for use as a shadow caster
            const float nearClipOffset = 100.0f;
            OrthographicCamera lightCamera = new OrthographicCamera(mins.X, maxes.X, mins.Y, maxes.Y, -maxes.Z - nearClipOffset, -mins.Z);
            lightCamera.View = viewMatrix;


            if (RenderDebug)
            {
                game.LineManager3D.AddCenteredBox(frustumCentroid - (light.Direction * distFromCentroid), 1.0f,
                                                   new Color4(0, 1, 0));
                game.LineManager3D.AddLine(frustumCentroid - (light.Direction * distFromCentroid), frustumCentroid,
                                            new Color4(1, 1, 0));
                game.LineManager3D.AddCenteredBox(frustumCentroid, 1.0f, new Color4(1, 0, 0));
                game.LineManager3D.AddViewFrustum(new BoundingFrustum(mainCamera.ViewProjection), new Color4(1, 0, 1));
                game.LineManager3D.AddViewFrustum(frustumCornersWS, new Color4(1, 0, 0));
                Vector3[] temps = new Vector3[8];
                Matrix tm = Matrix.Invert(viewMatrix);
                BoundingBox bb = new BoundingBox(mins, maxes);
                Vector3[] temps2 = bb.GetCorners();
                Vector3.TransformCoordinate(temps2, ref tm, temps);
                game.LineManager3D.AddViewFrustum(temps, new Color4(0, 1, 0));
                game.LineManager3D.AddViewFrustum(new BoundingFrustum(lightCamera.ViewProjection), new Color4(1, 0, 1));
            }

            return lightCamera;
        }


        /// <summary>
        /// Renders the shadow map using the orthographic camera created in
        /// CalculateFrustum.
        /// </summary>   
        protected void renderShadowMap(RenderPrimitives renderDelegate, int splitIndex)
        {
            // Set the viewport for the current split            
            Viewport splitViewport = new Viewport();
            splitViewport.MinZ = 0;
            splitViewport.MaxZ = 1;
            splitViewport.Width = ShadowMapSize;
            splitViewport.Height = ShadowMapSize;
            splitViewport.X = splitIndex * ShadowMapSize;
            splitViewport.Y = 0;

            context.Rasterizer.SetViewports(splitViewport);

            renderDelegate(lightCameras[splitIndex].ViewProjection);
        }

        /// <summary>
        /// Renders a texture containing the final shadow occlusion
        /// </summary>
        protected void RenderShadowOcclusion(ICamera mainCamera, ShaderResourceView depthTextureRV)
        {
            // Set the device to render to our shadow occlusion texture, and to use
            // the original DepthStencilSurface
            //graphicsDevice.SetRenderTarget( 0, shadowOcclusion );
            //graphicsDevice.DepthStencilBuffer = oldDS;

            Matrix cameraTransform;
            cameraTransform = mainCamera.ViewInverse;
            //mainCamera.GetWorldMatrix( out cameraTransform );

            // We'll use these clip planes to determine which split a pixel belongs to
            for (int i = 0; i < NumSplits; i++)
            {
                lightClipPlanes[i].X = -splitDepths[i];
                lightClipPlanes[i].Y = -splitDepths[i + 1];

                lightViewProjectionMatrices[i] = lightCameras[i].ViewProjection;
                //lightCameras[ i ].GetViewProjMatrix( out  );
            }

            // Setup the Effect
            shadowMapShader.SetTechnique(shadowOcclusionTechniques[(int)filteringType]);
            shadowMapShader.Effect.GetVariableByName("g_matInvView").AsMatrix().SetMatrix(cameraTransform);
            shadowMapShader.Effect.GetVariableByName("g_matLightViewProj").AsMatrix().SetMatrixArray(lightViewProjectionMatrices);
            //TODO: shadowMapShader.Effect.GetVariableByName( "g_vFrustumCornersVS" ).AsVector().Set ( farFrustumCornersVS );
            //TODO: shadowMapShader.Effect.GetVariableByName( "g_vClipPlanes" ).AsVector().Set( lightClipPlanes );
            shadowMapShader.Effect.GetVariableByName("ShadowMap").AsResource().SetResource(shadowMapRV);
            shadowMapShader.Effect.GetVariableByName("DepthTexture").AsResource().SetResource(depthTextureRV);
            //TODO: shadowMapShader.Effect.GetVariableByName( "g_vOcclusionTextureSize" ).SetValue( new Vector2( shadowOcclusion.Width, shadowOcclusion.Height ) );
            shadowMapShader.Effect.GetVariableByName("g_vShadowMapSize").AsVector().Set(new Vector2(shadowMap.Description.Width, shadowMap.Description.Height));
            shadowMapShader.Effect.GetVariableByName("g_bShowSplitColors").AsScalar().Set(showCascadeSplits);

            shadowMapShader.GetCurrentPass(0).Apply(context);
            // Draw the full screen quad		
            //TODO: fullScreenQuad.Draw();


            // Set to render to the back buffer
            //graphicsDevice.SetRenderTarget( 0, null );


            shadowMapShader.Effect.GetVariableByName("ShadowMap").AsResource().SetResource(null);
            shadowMapShader.Effect.GetVariableByName("DepthTexture").AsResource().SetResource(null);
            shadowMapShader.GetCurrentPass(0).Apply(context);

        }

    }
}
