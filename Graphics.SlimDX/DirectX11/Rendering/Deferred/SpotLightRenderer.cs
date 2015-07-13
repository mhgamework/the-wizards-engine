using System;
using DirectX11;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;

namespace MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Rendering.Deferred
{
    /// <summary>
    /// This draws light accumulation from a directional light (full screen)
    /// the rgb components contain diffuse, alpha contains specular
    /// This outputs on a normal single rendertarget
    /// </summary>
    public class SpotLightRenderer
    {
        private readonly DX11Game game;
        private readonly GBuffer gBuffer;
        private DeviceContext context;
        private BasicShader noShadowsShader;
        private BasicShader shadowsShader;

        private Texture2D shadowMap;
        private ShaderResourceView shadowMapRV;
        private DepthStencilView shadowMapDSV;



        private Vector3 color;
        private FullScreenQuad quad;
        private InputLayout layout;
        private int shadowMapSize = 1024;

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
        public Vector3 SpotDirection { get; set; }
        public float SpotLightAngle { get; set; }
        public float SpotDecayExponent { get; set; }
        public bool ShadowsEnabled { get; set; }

        public CustomCamera LightCamera { get; private set; }

        public ShaderResourceView ShadowMapRv
        {
            get { return shadowMapRV; }
            set { shadowMapRV = value; }
        }

        public SpotLightRenderer(DX11Game game, GBuffer gBuffer)
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
            SpotDirection = MathHelper.Down;
            SpotLightAngle = MathHelper.ToRadians(30);
            SpotDecayExponent = 1;


            Color = new Vector3(1, 1, 0.9f);



            shadowMap = new Texture2D(device, new Texture2DDescription
                                                  {
                                                      ArraySize = 1,
                                                      BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
                                                      CpuAccessFlags = CpuAccessFlags.None,
                                                      Format = global::SlimDX.DXGI.Format.R32_Typeless,
                                                      Width = shadowMapSize,
                                                      Height = shadowMapSize,
                                                      MipLevels = 1,
                                                      Usage = ResourceUsage.Default,
                                                      SampleDescription = new global::SlimDX.DXGI.SampleDescription(1, 0)
                                                  });
            shadowMapRV = new ShaderResourceView(device, shadowMap, new ShaderResourceViewDescription
                                                                        {
                                                                            Dimension = ShaderResourceViewDimension.Texture2D,
                                                                            Format = global::SlimDX.DXGI.Format.R32_Float,
                                                                            MipLevels = 1,
                                                                            MostDetailedMip = 0
                                                                        });
            shadowMapDSV = new DepthStencilView(device, shadowMap, new DepthStencilViewDescription
                                                                       {
                                                                           Dimension = DepthStencilViewDimension.Texture2D,
                                                                           Format = global::SlimDX.DXGI.Format.D32_Float
                                                                       });

            LightCamera = new CustomCamera();


            //var rasterizerInside = RasterizerState.FromDescription(device, new RasterizerStateDescription
            //                                                                   {
            //                                                                       CullMode = CullMode.Front
            //                                                                   });

            //var rasterizerOutside = RasterizerState.FromDescription(device, new RasterizerStateDescription
            //{
            //    CullMode = CullMode.Back
            //});




        }

        private void reloadShader(DX11Game game)
        {
            var fileName = CompiledShaderCache.Current.RootShaderPath + "Deferred\\SpotLight.fx";
            noShadowsShader = BasicShader.LoadAutoreload(game,
                                                         new System.IO.FileInfo(
                                                             fileName), null, new[] { new ShaderMacro("DISABLE_SHADOWS", "1") });

            noShadowsShader.SetTechnique("Technique0");

            shadowsShader = BasicShader.LoadAutoreload(game,
                                                  new System.IO.FileInfo(
                                                      fileName));

            shadowsShader.SetTechnique("Technique0");
        }

        public void Draw()
        {

            BasicShader shader;
            if (ShadowsEnabled)
            {
                shader = shadowsShader;
                shader.Effect.GetVariableByName("shadowMap").AsResource().SetResource(shadowMapRV);
                shader.Effect.GetVariableByName("LightViewProjection").AsMatrix().SetMatrix(
                    LightCamera.ViewProjection);
            }
            else
                shader = noShadowsShader;


            //compute the light world matrix
            //scale according to light radius, and translate it to light position

            shader.Effect.GetVariableByName("World").AsMatrix().SetMatrix(LightCamera.ViewInverse);
            shader.Effect.GetVariableByName("View").AsMatrix().SetMatrix(game.Camera.View);
            shader.Effect.GetVariableByName("Projection").AsMatrix().SetMatrix(game.Camera.Projection);


            float spotLightAngleCosine = (float)Math.Cos(SpotLightAngle);


            //light position
            shader.Effect.GetVariableByName("lightPosition").AsVector().Set(LightPosition);
            //set the color, radius and Intensity
            shader.Effect.GetVariableByName("Color").AsVector().Set(color);
            shader.Effect.GetVariableByName("lightRadius").AsScalar().Set(LightRadius);
            shader.Effect.GetVariableByName("lightIntensity").AsScalar().Set(LightIntensity);
            shader.Effect.GetVariableByName("spotDirection").AsVector().Set(SpotDirection);
            shader.Effect.GetVariableByName("spotLightAngleCosine").AsScalar().Set(spotLightAngleCosine);
            shader.Effect.GetVariableByName("spotDecayExponent").AsScalar().Set(SpotDecayExponent);
            //parameters for specular computations
            shader.Effect.GetVariableByName("cameraPosition").AsVector().Set(game.Camera.ViewInverse.GetTranslation());
            shader.Effect.GetVariableByName("InvertViewProjection").AsMatrix().SetMatrix(Matrix.Invert(game.Camera.View * game.Camera.Projection));
            //size of a halfpixel, for texture coordinates alignment
            //spotLightShader.Effect.GetVariableByName("halfPixel").AsVector().Set(halfPixel);
            shader.Effect.GetVariableByName("g_vShadowMapSize").AsVector().Set(new Vector2(shadowMapSize, shadowMapSize));

            //calculate the distance between the camera and light center
            float cameraToCenter = Vector3.Distance(game.Camera.ViewInverse.GetTranslation(), LightPosition);
            //if we are inside the light volume, draw the sphere's inside face
            //if (cameraToCenter < lightRadius)
            //    GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
            //else
            //    GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;



            shader.Effect.GetVariableByName("World").AsMatrix().SetMatrix(Matrix.Identity);
            shader.Effect.GetVariableByName("View").AsMatrix().SetMatrix(Matrix.Identity);
            shader.Effect.GetVariableByName("Projection").AsMatrix().SetMatrix(Matrix.Identity);
            //GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;


            gBuffer.SetToShader(shader);

            shader.Apply();
            //drawConePrimitives(40);
            quad.Draw(layout);

            gBuffer.UnsetFromShader(shader);
            shader.Effect.GetVariableByName("shadowMap").AsResource().SetResource(null);

            shader.Apply();



        }


        public void UpdateLightCamera()
        {


            Vector3 forward = MathHelper.Forward;
            Vector3 targetPlaneX = MathHelper.Right;
            Vector3 targetPlaneY = MathHelper.Up;

            var world = Matrix.Identity;

            world *= Matrix.Scaling(new Vector3(LightRadius, LightRadius, LightRadius));
            world *= Matrix.Scaling((float)Math.Tan(SpotLightAngle) * (targetPlaneX + targetPlaneY) + forward);
            Vector3 up = MathHelper.Up;
            if (Math.Abs(Vector3.Dot(up, SpotDirection)) > 0.999)
                up = MathHelper.Right;


            world *= Matrix.Invert(Matrix.LookAtRH(Vector3.Zero, -SpotDirection, up));//world *= Matrix.CreateWorld(Vector3.Zero, -SpotDirection, up);
            world *= Matrix.Translation(LightPosition);


            Vector3 scale, translation;
            Quaternion rotation;
            world.Decompose(out scale, out rotation, out translation);

            Matrix view = Matrix.Invert(world);
            Matrix projection = Matrix.PerspectiveFovRH(MathHelper.PiOver2, 1, 0.01f, 1); //TODO: fix field of view?
            LightCamera.SetViewProjectionMatrix(view, projection);
            LightCamera.NearClip = 0.01f;
            LightCamera.FarClip = 1;
        }

        public void UpdateShadowMap(Action renderScene)
        {
            context.ClearState();
            context.ClearDepthStencilView(shadowMapDSV, DepthStencilClearFlags.Depth, 1, 0);
            context.Rasterizer.SetViewports(new Viewport(0, 0, shadowMapSize, shadowMapSize));
            context.OutputMerger.SetTargets(shadowMapDSV);

            renderScene();
        }
    }
}
