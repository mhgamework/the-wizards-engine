using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;
using SlimDX;
using SlimDX.Direct3D11;

namespace MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Rendering.Deferred
{
    public class FogEffect
    {
        private readonly DX11Game game;
        private DeviceContext context;
        private BasicShader shader;


        private FullScreenQuad quad;
        private InputLayout layout;
        private RenderTargetView averageLumRTV;
        private ShaderResourceView averageLumRV;

        public FogEffect(DX11Game game)
        {
            this.game = game;
            var device = game.Device;
            context = device.ImmediateContext;

            shader = BasicShader.LoadAutoreload(game,
                                                new System.IO.FileInfo(
                                                    CompiledShaderCache.Current.RootShaderPath + "Deferred\\Fog.fx"));

            shader.SetTechnique("Technique0");

            quad = new FullScreenQuad(device);

            layout = FullScreenQuad.CreateInputLayout(device, shader.GetCurrentPass(0));

        }


        public void PostProcessFog(ShaderResourceView input, GBuffer gBuffer, RenderTargetView target)
        {
            global::SlimDX.Performance.BeginEvent(new Color4(), "Fog!");
            context.OutputMerger.SetTargets(target);
            shader.Effect.GetVariableByName("inputMap").AsResource().SetResource(input);
            shader.Effect.GetVariableByName("InvertProjection").AsMatrix().SetMatrix(
                Matrix.Invert(game.Camera.Projection));
            
            gBuffer.SetToShader(shader);


            shader.Apply();
            quad.Draw(layout);

            shader.Effect.GetVariableByName("inputMap").AsResource().SetResource(null);
            shader.Apply();

            global::SlimDX.Performance.EndEvent();
        }
    }
}