using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX;
using SlimDX.DirectInput;


namespace MHGameWork.TheWizards.Engine.WorldRendering
{
    /// <summary>
    /// Responsible for rendering the world
    /// NOTE: UI simulation was moved the UISimualtor
    /// </summary>
    public class WorldRenderingSimulator : ISimulator
    {
        private DeferredRenderer deferred;
        private CameraInfo info;
        
        private WireframeBoxSimulator wireframeSimulator;
        private EntitySimulator entitySimulator;
        private PointLightSimulator pointLightSimulator;

        public WorldRenderingSimulator()
        {

            deferred = TW.Graphics.AcquireRenderer();

            //deferred.SSAO.MRadiusMultiplier = 10;
            //deferred.SSAO.MNumDirs = 8;
            //deferred.SSAO.MNumSteps = 2;
            //deferred.SSAO.MContrast = 2;
            deferred.SSAO.MRadiusMultiplier = 0.5f;
            deferred.PointLightRenderer.ShadowUpdateInterval = 240;

            var data = TW.Data.GetSingleton<Data>();

            {
                var light = deferred.CreateDirectionalLight();
                light.LightDirection = Vector3.Normalize(new Vector3(1, -1, 1));
                light.ShadowsEnabled = true;
                data.LightCreated = true;

                //var light = deferred.CreateSpotLight();
                //light.LightPosition = new Vector3(-1,1,-1) *30;
                //light.LightIntensity = 0.1f;
                //if (data.SunEnabled)
                //  light.LightIntensity = 1;
                //light.LightRadius = 400;
                //light.SpotDirection = Vector3.Normalize(-light.LightPosition);
                //light.ShadowsEnabled = true;
                

            }



            info = TW.Data.GetSingleton<CameraInfo>();



            entitySimulator = new EntitySimulator();
            wireframeSimulator = new WireframeBoxSimulator();
            pointLightSimulator = new PointLightSimulator();



        }

        public void Simulate()
        {

            if (TW.Graphics.Keyboard.IsKeyPressed(Key.Z))
                TW.Graphics.AcquireRenderer().Wireframe = !TW.Graphics.AcquireRenderer().Wireframe; entitySimulator.Simulate();


            wireframeSimulator.Simulate();
            pointLightSimulator.Simulate();


            drawDeferred();

            renderLines();
        }
        
        [TWProfile]
        private void drawDeferred()
        {
            TW.Graphics.Camera = info.ActiveCamera;
            deferred.Draw(); // TODO: fixthis : LOL draw first ? is this correct?
        }

        
        private static void renderLines()
        {
            TW.Graphics.LineManager3D.Render(TW.Graphics.Camera);
            TW.Graphics.SetBackbuffer();
        }

        public class Data : EngineModelObject
        {
            public bool LightCreated = false;

            public bool SunEnabled = true;
        }
    }
}
