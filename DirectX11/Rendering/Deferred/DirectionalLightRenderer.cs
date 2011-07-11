using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11.Graphics;
using SlimDX;
using SlimDX.Direct3D11;

namespace DirectX11.Rendering.Deferred
{
    /// <summary>
    /// This draws light accumulation from a directional light (full screen)
    /// the rgb components contain diffuse, alpha contains specular
    /// </summary>
    public class DirectionalLightRenderer
    {
        private readonly DX11Game game;
        private readonly GBuffer gBuffer;
        private DeviceContext context;
        private BasicShader shader;

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

        public DirectionalLightRenderer(DX11Game game, GBuffer gBuffer)
        {
            this.game = game;
            this.gBuffer = gBuffer;
            var device = game.Device;
            context = device.ImmediateContext;

            shader = BasicShader.LoadAutoreload(game,
                                                new System.IO.FileInfo(
                                                    "..\\..\\DirectX11\\Shaders\\Deferred\\DirectionalLight.fx"));

            shader.SetTechnique("Technique0");

            quad = new FullScreenQuad(device);

            layout = FullScreenQuad.CreateInputLayout(device, shader.GetCurrentPass(0));

            LightDirection = Vector3.Normalize(new Vector3(1, 2, 1));
            Color = new Vector3(1, 1, 0.9f);


        }

        public void Draw()
        {
            shader.Effect.GetVariableByName("cameraPosition").AsVector().Set(game.Camera.ViewInverse.GetTranslation());
            shader.Effect.GetVariableByName("InvertViewProjection").AsMatrix().SetMatrix(Matrix.Invert( game.Camera.ViewProjection));
            shader.Effect.GetVariableByName("lightDirection").AsVector().Set(lightDirection);
            shader.Effect.GetVariableByName("Color").AsVector().Set(color);

            gBuffer.SetToShader(shader);

            shader.Apply();
            quad.Draw(layout);

            gBuffer.UnsetFromShader(shader);

            shader.Apply();



        }
    }
}
