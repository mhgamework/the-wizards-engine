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
    public class SpotLightRenderer
    {
        private readonly DX11Game game;
        private readonly GBuffer gBuffer;
        private DeviceContext context;
        private BasicShader spotLightShader;


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
        public Vector3 SpotDirection { get; set; }
        public float SpotLightAngle { get; set; }
        public float SpotDecayExponent { get; set; }
        
        public SpotLightRenderer(DX11Game game, GBuffer gBuffer)
        {
            this.game = game;
            this.gBuffer = gBuffer;
            var device = game.Device;
            context = device.ImmediateContext;

            spotLightShader = BasicShader.LoadAutoreload(game,
                                                new System.IO.FileInfo(
                                                    "..\\..\\DirectX11\\Shaders\\Deferred\\SpotLight.fx"));

            spotLightShader.SetTechnique("Technique0");

            quad = new FullScreenQuad(device);

            layout = FullScreenQuad.CreateInputLayout(device, spotLightShader.GetCurrentPass(0));

            LightPosition = new Vector3(0, 6, 0);
            LightRadius = 6;
            LightIntensity = 1;
            SpotDirection = MathHelper.Down;
            SpotLightAngle = MathHelper.ToRadians(30);
            SpotDecayExponent = 1;


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


            float spotLightAngleCosine = (float)Math.Cos(SpotLightAngle);
            Vector3 forward = MathHelper .Forward;
            Vector3 targetPlaneX = MathHelper.Right;
            Vector3 targetPlaneY = MathHelper.Up;

            var world = Matrix.Identity;

            world *= Matrix.Scaling(new Vector3(LightRadius, LightRadius, LightRadius));
            world *= Matrix.Scaling((float)Math.Tan(SpotLightAngle) * (targetPlaneX + targetPlaneY) + forward);
            Vector3 up = MathHelper.Up;
            if (Math.Abs(Vector3.Dot(up, SpotDirection)) > 0.999)
                up = MathHelper.Right;

            //TODO: world *= Matrix.CreateWorld(Vector3.Zero, -spotDirection, up);
            world *= Matrix.Translation(LightPosition);


            //clearRenderStatesLightAccumulation();

            ////TODO: var LightViewProjection = drawSpotLightShadowMap(lightPosition, color, lightRadius, lightIntensity, spotDirection, spotLightAngle, spotDecayExponent, world);

            //setRenderStatesLightAccumulation();

            //if (OutputMode == DeferredOutputMode.ShadowMap)
            //    return;


            
            //spotLightShader.Effect.GetVariableByName("shadowMap").SetValue(shadowMapRT.GetTexture());
            //compute the light world matrix
            //scale according to light radius, and translate it to light position

            spotLightShader.Effect.GetVariableByName("World").AsMatrix().SetMatrix(world);
            spotLightShader.Effect.GetVariableByName("View").AsMatrix().SetMatrix(game.Camera.View);
            spotLightShader.Effect.GetVariableByName("Projection").AsMatrix().SetMatrix(game.Camera.Projection);



            //spotLightShader.Effect.GetVariableByName("LightViewProjection").AsMatrix().SetMatrix(LightViewProjection);

            //light position
            spotLightShader.Effect.GetVariableByName("lightPosition").AsVector().Set(LightPosition);
            //set the color, radius and Intensity
            spotLightShader.Effect.GetVariableByName("Color").AsVector().Set(color);
            spotLightShader.Effect.GetVariableByName("lightRadius").AsScalar().Set(LightRadius);
            spotLightShader.Effect.GetVariableByName("lightIntensity").AsScalar().Set(LightIntensity);
            spotLightShader.Effect.GetVariableByName("spotDirection").AsVector().Set(SpotDirection);
            spotLightShader.Effect.GetVariableByName("spotLightAngleCosine").AsScalar().Set(spotLightAngleCosine);
            spotLightShader.Effect.GetVariableByName("spotDecayExponent").AsScalar().Set(SpotDecayExponent);
            //parameters for specular computations
            spotLightShader.Effect.GetVariableByName("cameraPosition").AsVector().Set(game.Camera.ViewInverse.GetTranslation());
            spotLightShader.Effect.GetVariableByName("InvertViewProjection").AsMatrix().SetMatrix(Matrix.Invert(game.Camera.View * game.Camera.Projection));
            //size of a halfpixel, for texture coordinates alignment
            //spotLightShader.Effect.GetVariableByName("halfPixel").AsVector().Set(halfPixel);
            //spotLightShader.Effect.GetVariableByName("g_vShadowMapSize").AsVector().Set(new Vector2(shadowMapSize, shadowMapSize));

            //calculate the distance between the camera and light center
            float cameraToCenter = Vector3.Distance(game.Camera.ViewInverse.GetTranslation(), LightPosition);
            //if we are inside the light volume, draw the sphere's inside face
            //if (cameraToCenter < lightRadius)
            //    GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
            //else
            //    GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;



            spotLightShader.Effect.GetVariableByName("World").AsMatrix().SetMatrix(Matrix.Identity);
            spotLightShader.Effect.GetVariableByName("View").AsMatrix().SetMatrix(Matrix.Identity);
            spotLightShader.Effect.GetVariableByName("Projection").AsMatrix().SetMatrix(Matrix.Identity);
            //GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;


            gBuffer.SetToShader(spotLightShader);

            spotLightShader.Apply();
            //drawConePrimitives(40);
            quad.Draw(layout);

            gBuffer.UnsetFromShader(spotLightShader);

            spotLightShader.Apply();



        }
    }
}
