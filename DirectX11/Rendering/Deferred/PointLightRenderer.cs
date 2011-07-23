using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11.Graphics;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;

namespace DirectX11.Rendering.Deferred
{
    /// <summary>
    /// This draws light accumulation from a directional light (full screen)
    /// the rgb components contain diffuse, alpha contains specular
    /// </summary>
    public class PointLightRenderer : IDisposable
    {
        private readonly DX11Game game;
        private readonly GBuffer gBuffer;
        private DeviceContext context;
        private BasicShader noShadowsShader;


        private Vector3 color;
        private FullScreenQuad quad;
        private InputLayout layout;

        public Vector3 Color
        {
            get { return color; }
            set
            {
                color = value;
            }
        }

        public Vector3 LightPosition { get; set; }
        public float LightIntensity { get; set; }
        public float LightRadius { get; set; }
        private int shadowMapSize = 1024;
        private BasicShader shadowsShader;
        private ShaderResourceView[] shadowCubeMapRVs;
        private DepthStencilView[] depthStencilViewFaces;
        private Matrix shadowMapProjection;

        public PointLightRenderer(DX11Game game, GBuffer gBuffer)
        {
            this.game = game;
            this.gBuffer = gBuffer;
            var device = game.Device;
            context = device.ImmediateContext;

            reloadShader(game);

            quad = new FullScreenQuad(device);

            layout = FullScreenQuad.CreateInputLayout(device, noShadowsShader.GetCurrentPass(0));

            LightPosition = new Vector3(0, 6, 0);
            LightRadius = 6;
            LightIntensity = 1;


            Color = new Vector3(1, 1, 0.9f);

            //var rasterizerInside = RasterizerState.FromDescription(device, new RasterizerStateDescription
            //                                                                   {
            //                                                                       CullMode = CullMode.Front
            //                                                                   });

            //var rasterizerOutside = RasterizerState.FromDescription(device, new RasterizerStateDescription
            //{
            //    CullMode = CullMode.Back
            //});

            var shadowCubeMap = new Texture2D(device, new Texture2DDescription
                                                          {
                                                              ArraySize = 6,
                                                              BindFlags =
                                                                  BindFlags.DepthStencil | BindFlags.ShaderResource,
                                                              CpuAccessFlags = CpuAccessFlags.None,
                                                              Format = SlimDX.DXGI.Format.R32_Typeless,
                                                              Height = shadowMapSize,
                                                              Width = shadowMapSize,
                                                              MipLevels = 1,
                                                              OptionFlags = ResourceOptionFlags.TextureCube,
                                                              SampleDescription =
                                                                  new SlimDX.DXGI.SampleDescription(1, 0),
                                                              Usage = ResourceUsage.Default
                                                          });
            depthStencilViewFaces = new DepthStencilView[6];
            shadowCubeMapRVs = new ShaderResourceView[6];
            LightCameras = new CustomCamera[6];

            for (int i = 0; i < 6; i++)
            {
                depthStencilViewFaces[i] = new DepthStencilView(device, shadowCubeMap, new DepthStencilViewDescription
                                                                                           {
                                                                                               Dimension =
                                                                                                   DepthStencilViewDimension
                                                                                                   .Texture2DArray,
                                                                                               ArraySize = 1,
                                                                                               FirstArraySlice = i,
                                                                                               Flags =
                                                                                                   DepthStencilViewFlags
                                                                                                   .None,
                                                                                               Format =
                                                                                                   SlimDX.DXGI.Format.
                                                                                                   D32_Float,
                                                                                               MipSlice = 0
                                                                                           });

                //ShadowCubeMapRVs[i] = new ShaderResourceView(device, shadowCubeMap, new ShaderResourceViewDescription
                //{
                //    Dimension = ShaderResourceViewDimension.Texture2D,
                //    ArraySize = 1,
                //    FirstArraySlice = i,
                //    Format = SlimDX.DXGI.Format.R32_Float,
                //    MipLevels = 1,
                //    MostDetailedMip = 0
                //});
                LightCameras[i] = new CustomCamera();
            }

            ShadowCubeMapRv = new ShaderResourceView(device, shadowCubeMap, new ShaderResourceViewDescription
                                                                                {
                                                                                    Dimension = ShaderResourceViewDimension.TextureCube,
                                                                                    MostDetailedMip = 0,
                                                                                    MipLevels = 1,
                                                                                    Format = SlimDX.DXGI.Format.R32_Float
                                                                                });


        }

        private void reloadShader(DX11Game game)
        {
            noShadowsShader = BasicShader.LoadAutoreload(game,
                                                          new System.IO.FileInfo(
                                                              "..\\..\\DirectX11\\Shaders\\Deferred\\PointLight.fx"), null, new[] { new ShaderMacro("DISABLE_SHADOWS") });

            noShadowsShader.SetTechnique("Technique0");

            shadowsShader = BasicShader.LoadAutoreload(game,
                                              new System.IO.FileInfo(
                                                  "..\\..\\DirectX11\\Shaders\\Deferred\\PointLight.fx"));

            shadowsShader.SetTechnique("Technique0");
        }

        public void Draw()
        {
            BasicShader shader;
            if (ShadowsEnabled)
            {
                shader = shadowsShader;
                shader.Effect.GetVariableByName("shadowMap").AsResource().SetResource(ShadowCubeMapRv);
                shader.Effect.GetVariableByName("ShadowMapProjection").AsMatrix().SetMatrix(shadowMapProjection);
            }
            else
                shader = noShadowsShader;

            //compute the light world matrix
            //scale according to light radius, and translate it to light position
            Matrix sphereWorldMatrix = Matrix.Scaling(new Vector3(1, 1, 1) * LightRadius) * Matrix.Translation(LightPosition);
            shader.Effect.GetVariableByName("World").AsMatrix().SetMatrix(sphereWorldMatrix);
            shader.Effect.GetVariableByName("View").AsMatrix().SetMatrix(game.Camera.View);
            shader.Effect.GetVariableByName("Projection").AsMatrix().SetMatrix(game.Camera.Projection);
            //light position
            shader.Effect.GetVariableByName("lightPosition").AsVector().Set(LightPosition);
            //set the color, radius and Intensity
            shader.Effect.GetVariableByName("Color").AsVector().Set(color);
            shader.Effect.GetVariableByName("lightRadius").AsScalar().Set(LightRadius);
            shader.Effect.GetVariableByName("lightIntensity").AsScalar().Set(LightIntensity);
            //parameters for specular computations
            shader.Effect.GetVariableByName("cameraPosition").AsVector().Set(game.Camera.ViewInverse.GetTranslation());
            shader.Effect.GetVariableByName("InvertViewProjection").AsMatrix().SetMatrix(Matrix.Invert(game.Camera.View * game.Camera.Projection));
            //size of a halfpixel, for texture coordinates alignment
            //pointLightShader.Effect.GetVariableByName("halfPixel").AsVector().Set(halfPixel);

            //calculate the distance between the camera and light center
            float cameraToCenter = Vector3.Distance(game.Camera.ViewInverse.GetTranslation(), LightPosition);
            //if we are inside the light volume, draw the sphere's inside face
            //if (cameraToCenter < lightRadius)
            //    GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
            //else
            //    GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

            
            gBuffer.SetToShader(shader);

            shader.Apply();
            //drawSpherePrimitives();
            quad.Draw(layout);

            gBuffer.UnsetFromShader(shader);

            shader.Apply();



        }



        public delegate void ShadowMapRenderDelegate(CustomCamera lightCamera);
        public void UpdateShadowMap(ShadowMapRenderDelegate renderScene)
        {
            shadowMapProjection = Matrix.PerspectiveFovLH(MathHelper.PiOver2, 1, 1f, LightRadius+1 );
            for (int i = 0; i < 6; i++)
            {
                Performance.BeginEvent(new Color4(1, 1, 0), "ShadowCubeFace" + i.ToString());
                Vector3 vLookDirection;
                Vector3 vUpVec;

                switch (i)
                {
                    case 0:
                        vLookDirection = new Vector3(1.0f, 0.0f, 0.0f);
                        vUpVec = new Vector3(0.0f, 1.0f, 0.0f);
                        break;
                    case 1:
                        vLookDirection = new Vector3(-1.0f, 0.0f, 0.0f);
                        vUpVec = new Vector3(0.0f, 1.0f, 0.0f);
                        break;
                    case 2:
                        vLookDirection = new Vector3(0.0f, 1.0f, 0.0f);
                        vUpVec = new Vector3(0.0f, 0.0f, -1.0f);
                        break;
                    case 3:
                        vLookDirection = new Vector3(0.0f, -1.0f, 0.0f);
                        vUpVec = new Vector3(0.0f, 0.0f, 1.0f);
                        break;
                    case 4:
                        vLookDirection = new Vector3(0.0f, 0.0f, 1.0f);
                        vUpVec = new Vector3(0.0f, 1.0f, 0.0f);
                        break;
                    case 5:
                        vLookDirection = new Vector3(0.0f, 0.0f, -1.0f);
                        vUpVec = new Vector3(0.0f, 1.0f, 0.0f);
                        break;
                    default:
                        vLookDirection = new Vector3();
                        vUpVec = new Vector3();
                        break;
                }

                var view = Matrix.LookAtLH(LightPosition, LightPosition + vLookDirection, vUpVec); //WAS: LH

                context.ClearDepthStencilView(depthStencilViewFaces[i], DepthStencilClearFlags.Depth, 1, 0);

                //if (i != 0) continue;

                //context.ClearDepthStencilView(depthStencilViewFaces[i], DepthStencilClearFlags.Depth, 0, 0);
                LightCameras[i].SetViewProjectionMatrix(view, shadowMapProjection);
                context.ClearState();
                context.Rasterizer.SetViewports(new Viewport(0, 0, shadowMapSize, shadowMapSize));
                context.OutputMerger.SetTargets(depthStencilViewFaces[i]);
                renderScene(LightCameras[i]);

                Performance.EndEvent();
            }
        }

        public bool ShadowsEnabled { get; set; }

        /// <summary>
        /// WARNING: THIS DOES NOT WORK IN D3D 10.0
        /// </summary>
        public ShaderResourceView[] ShadowCubeMapRVs
        {
            get { return shadowCubeMapRVs; }
        }

        public CustomCamera[] LightCameras { get; private set; }

        public ShaderResourceView ShadowCubeMapRv { get; private set; }


        public void Dispose()
        {
            noShadowsShader.Dispose();
            layout.Dispose();
            quad.Dispose();
        }
    }
}
