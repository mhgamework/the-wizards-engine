using DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.DirectX11.Rendering.CSM;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;

namespace MHGameWork.TheWizards.DirectX11.Rendering.Deferred
{
    /// <summary>
    /// This draws light accumulation from a directional light (full screen)
    /// the rgb components contain diffuse, alpha contains specular
    /// This outputs onto a normal single render target
    /// </summary>
    public class DirectionalLightRenderer
    {
        private readonly DX11Game game;
        private readonly GBuffer gBuffer;
        private DeviceContext context;
        private BasicShader shadowsShader;

        private Vector3 lightDirection;
        public Vector3 LightDirection
        {
            get { return lightDirection; }
            set
            {
                lightDirection = value;
            }
        }

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

        public bool ShadowsEnabled { get; set; }

        public CSMRenderer CSMRenderer { get; private set; }


        public DirectionalLightRenderer(DX11Game game, GBuffer gBuffer)
        {
            this.game = game;
            this.gBuffer = gBuffer;
            var device = game.Device;
            context = device.ImmediateContext;

            shadowsShader = BasicShader.LoadAutoreload(game,
                                                new System.IO.FileInfo(
                                                    CompiledShaderCache.Current.RootShaderPath + "Deferred\\DirectionalLight.fx"));

            shadowsShader.SetTechnique("Technique0");

            noShadowsShader = BasicShader.LoadAutoreload(game,
                                                         new System.IO.FileInfo(
                                                             CompiledShaderCache.Current.RootShaderPath + "Deferred\\DirectionalLight.fx"), null, new[] { new ShaderMacro("DISABLE_SHADOWS") });

            noShadowsShader.SetTechnique("Technique0");

            quad = new FullScreenQuad(device);

            layout = FullScreenQuad.CreateInputLayout(device, shadowsShader.GetCurrentPass(0));

            LightDirection = Vector3.Normalize(new Vector3(1, 2, 1));
            Color = new Vector3(1, 1, 0.9f);

            CSMRenderer = new CSM.CSMRenderer(game);


        }

        private DirectionalLight csmLight = new DirectionalLight();
        private BasicShader noShadowsShader;

        /// <summary>
        /// ??
        /// </summary>
        /// <param name="renderDelegate"></param>
        /// <param name="mainCamera"></param>
        public void UpdateShadowmap(CSM.CSMRenderer.RenderPrimitives renderDelegate, ICamera mainCamera)
        {
            csmLight.Direction = LightDirection;
            CSMRenderer.UpdateShadowMap(renderDelegate, csmLight, mainCamera);
        }


        /// <summary>
        /// 
        /// </summary>
        public void Draw()
        {
            BasicShader shader;

            if (ShadowsEnabled)
            {
                shader = shadowsShader;

                CSMRenderer.SetShadowOcclusionShaderVariables(shader, game.Camera);
                shader.Effect.GetVariableByName("InvertProjection").AsMatrix().SetMatrix(
                    Matrix.Invert(game.Camera.Projection));
            }
            else
                shader = noShadowsShader;

            shader.Effect.GetVariableByName("cameraPosition").AsVector().Set(game.Camera.ViewInverse.GetTranslation());
            shader.Effect.GetVariableByName("InvertViewProjection").AsMatrix().SetMatrix(Matrix.Invert(game.Camera.ViewProjection));
            shader.Effect.GetVariableByName("lightDirection").AsVector().Set(lightDirection);
            shader.Effect.GetVariableByName("Color").AsVector().Set(color);

            gBuffer.SetToShader(shader);

            shader.Apply();
            quad.Draw(layout);

            gBuffer.UnsetFromShader(shader);
            if (ShadowsEnabled)
                CSMRenderer.UnSetShadowOcclusionShaderVariables(shader);


            shader.Apply();





        }
    }
}
