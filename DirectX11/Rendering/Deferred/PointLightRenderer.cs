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
    public class PointLightRenderer : IDisposable
    {
        private readonly DX11Game game;
        private readonly GBuffer gBuffer;
        private DeviceContext context;
        private BasicShader pointLightShader;


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

        public PointLightRenderer(DX11Game game, GBuffer gBuffer)
        {
            this.game = game;
            this.gBuffer = gBuffer;
            var device = game.Device;
            context = device.ImmediateContext;

            pointLightShader = BasicShader.LoadAutoreload(game,
                                                new System.IO.FileInfo(
                                                    "..\\..\\DirectX11\\Shaders\\Deferred\\PointLight.fx"));

            pointLightShader.SetTechnique("Technique0");

            quad = new FullScreenQuad(device);

            layout = FullScreenQuad.CreateInputLayout(device, pointLightShader.GetCurrentPass(0));

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




        }

        public void Draw()
        {


            //compute the light world matrix
            //scale according to light radius, and translate it to light position
            Matrix sphereWorldMatrix = Matrix.Scaling(new Vector3(1, 1, 1) * LightRadius) * Matrix.Translation(LightPosition);
            pointLightShader.Effect.GetVariableByName("World").AsMatrix().SetMatrix(sphereWorldMatrix);
            pointLightShader.Effect.GetVariableByName("View").AsMatrix().SetMatrix(game.Camera.View);
            pointLightShader.Effect.GetVariableByName("Projection").AsMatrix().SetMatrix(game.Camera.Projection);
            //light position
            pointLightShader.Effect.GetVariableByName("lightPosition").AsVector().Set(LightPosition);
            //set the color, radius and Intensity
            pointLightShader.Effect.GetVariableByName("Color").AsVector().Set(color);
            pointLightShader.Effect.GetVariableByName("lightRadius").AsScalar().Set(LightRadius);
            pointLightShader.Effect.GetVariableByName("lightIntensity").AsScalar().Set(LightIntensity);
            //parameters for specular computations
            pointLightShader.Effect.GetVariableByName("cameraPosition").AsVector().Set(game.Camera.ViewInverse.GetTranslation());
            pointLightShader.Effect.GetVariableByName("InvertViewProjection").AsMatrix().SetMatrix(Matrix.Invert(game.Camera.View * game.Camera.Projection));
            //size of a halfpixel, for texture coordinates alignment
            //pointLightShader.Effect.GetVariableByName("halfPixel").AsVector().Set(halfPixel);

            //calculate the distance between the camera and light center
            float cameraToCenter = Vector3.Distance(game.Camera.ViewInverse.GetTranslation(), LightPosition);
            //if we are inside the light volume, draw the sphere's inside face
            //if (cameraToCenter < lightRadius)
            //    GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
            //else
            //    GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;


            gBuffer.SetToShader(pointLightShader);

            pointLightShader.Apply();
            //drawSpherePrimitives();
            quad.Draw(layout);

            gBuffer.UnsetFromShader(pointLightShader);

            pointLightShader.Apply();



        }


        public void Dispose()
        {
            pointLightShader.Dispose();
            layout.Dispose();
            quad.Dispose();
        }
    }
}
