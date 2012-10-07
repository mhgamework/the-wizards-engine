using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;



namespace MHGameWork.TheWizards.Simulators
{
    /// <summary>
    /// Responsible for rendering using a basic rendering system.
    /// </summary>
    public class WorldRenderingSimulator : ISimulator
    {
        private DeferredRenderer deferred;
        private WorldRenderer renderer;
        private CameraInfo info;
        private TextareaUpdater textareaUpdater;

        public WorldRenderingSimulator()
        {

            deferred = TW.Graphics.AcquireRenderer();
            renderer = new WorldRenderer(TW.Data, deferred);

            var data = TW.Data.GetSingleton<Data>();

            if (!data.LightCreated)
            {
                var light = deferred.CreateDirectionalLight();
                light.LightDirection = Vector3.Normalize(new Vector3(1, -1, 1));
                light.ShadowsEnabled = true;
                data.LightCreated = true;
            }



            info = TW.Data.GetSingleton<CameraInfo>();


            textareaUpdater = new TextareaUpdater();



        }

        public void Simulate()
        {
            TW.Graphics.Camera = info.ActiveCamera;
            renderer.ProcessWorldChanges();
            deferred.Draw();

            textareaUpdater.Update();
            textareaUpdater.Render();

        }

        public class Data : EngineModelObject
        {
            public bool LightCreated = false;
        }
    }
}
