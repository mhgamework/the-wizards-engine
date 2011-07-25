using System.Collections.Generic;
using DirectX11;
using DirectX11.Graphics;
using DirectX11.Rendering.CSM;
using DirectX11.Rendering.Deferred;
using SlimDX;

namespace MHGameWork.TheWizards.Rendering.Deferred
{
    /// <summary>
    /// This is the facade class for the Deferred Renderer. This hides the DirectX11 layer.
    /// </summary>
    public class DeferredRenderer
    {
        private readonly DX11Game game;
        private DeferredMeshRenderer meshRenderer;
        private DirectionalLightRenderer directionalLightRenderer;
        private SpotLightRenderer spotLightRenderer;
        private PointLightRenderer pointLightRenderer;

        private List<DirectionalLight> directionalLights = new List<DirectionalLight>();
        private List<PointLight> pointLights = new List<PointLight>();
        private List<SpotLight> spotLights = new List<SpotLight>();
        private CombineFinalRenderer combineFinalRenderer;
        private GBuffer gBuffer;
        private TexturePool texturePool;


        public DeferredRenderer(DX11Game game)
        {
            this.game = game;

            int width = 800;
            int height = 600;

            gBuffer = new GBuffer(game.Device, width, height);
            texturePool = new TexturePool(game);

            meshRenderer = new DeferredMeshRenderer(game, gBuffer, texturePool);

            directionalLightRenderer = new DirectionalLightRenderer(game, gBuffer);
            spotLightRenderer = new SpotLightRenderer(game, gBuffer);
            pointLightRenderer = new PointLightRenderer(game, gBuffer);

            combineFinalRenderer = new CombineFinalRenderer(game, gBuffer);


        }

        public DirectionalLight CreateDirectionalLight()
        {
            var light = new DirectionalLight();
            directionalLights.Add(light);
            return light;
        }

        public PointLight CreatePointLight()
        {
            var light = new PointLight();
            pointLights.Add(light);
            return light;
        }

        public SpotLight CreateSpotLight()
        {
            var light = new SpotLight();
            spotLights.Add(light);
            return light;
        }

        public DeferredMeshRenderElement CreateMeshElement(IMesh mesh)
        {
            return meshRenderer.AddMesh(mesh);
        }

        public void Draw()
        {
            gBuffer.Clear();
            gBuffer.SetTargetsToOutputMerger();
            meshRenderer.Draw();

            combineFinalRenderer.ClearLightAccumulation();

            // Possibly do all shadowmap up front, even cache some shadow maps.

            for (int i = 0; i < directionalLights.Count; i++)
            {
                var l = directionalLights[i];
                var r = directionalLightRenderer;
                r.LightDirection = l.LightDirection;
                r.Color = l.Color;
                r.ShadowsEnabled = l.ShadowsEnabled;

                if (l.ShadowsEnabled)
                    updateDirectionalShadows(r);
                combineFinalRenderer.SetLightAccumulationStates();

                r.Draw();
            }

            for (int i = 0; i < pointLights.Count; i++)
            {
                var l = pointLights[i];
                var r = pointLightRenderer;
                r.LightIntensity = l.LightIntensity;
                r.LightPosition = l.LightPosition;
                r.LightRadius = l.LightRadius;
                r.Color = l.Color;
                r.ShadowsEnabled = l.ShadowsEnabled;
                
                if (l.ShadowsEnabled)
                    updatePointShadows(r);

                combineFinalRenderer.SetLightAccumulationStates();

                r.Draw();
            }

            for (int i = 0; i < spotLights.Count; i++)
            {
                var l = spotLights[i];
                var r = spotLightRenderer;
                r.LightIntensity = l.LightIntensity;
                r.LightPosition = l.LightPosition;
                r.LightRadius = l.LightRadius;
                r.SpotDecayExponent = l.SpotDecayExponent;
                r.SpotDirection = l.SpotDirection;
                r.SpotLightAngle = l.SpotLightAngle;
                r.Color = l.Color;
                r.ShadowsEnabled = l.ShadowsEnabled;

                if (l.ShadowsEnabled)
                {
                    r.UpdateLightCamera();
                    game.Camera = r.LightCamera;
                    r.UpdateShadowMap(meshRenderer.Draw);
                    game.Camera = game.SpecaterCamera;
                }

                combineFinalRenderer.SetLightAccumulationStates();

                r.Draw();
            }

            game.Device.ImmediateContext.ClearState();
            game.SetBackbuffer();

            combineFinalRenderer.DrawCombined();

            //game.TextureRenderer.Draw(pointLightRenderer.ShadowCubeMapRv, new Vector2(10, 10), new Vector2(300, 300));

        }

        private void updatePointShadows(PointLightRenderer r)
        {
            r.UpdateShadowMap(delegate(CustomCamera lightCamera)
                                  {
                                      game.Camera = lightCamera;
                                      meshRenderer.Draw();
                                      game.Camera = game.SpecaterCamera;
                                  });
        }

        private void updateDirectionalShadows(DirectionalLightRenderer r)
        {
            r.DrawUpdatedShadowmap(delegate(OrthographicCamera lightCamera)
                                       {
                                           game.Camera = lightCamera;
                                           meshRenderer.Draw();
                                           game.Camera = game.SpecaterCamera;
                                       }, game.Camera);
        }
    }
}
