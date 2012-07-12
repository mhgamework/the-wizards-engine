using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards.Simulators
{
    /// <summary>
    /// Responsible for rendering using a basic rendering system.
    /// </summary>
    public class RenderingSimulator : ISimulator
    {
        private DeferredRenderer deferred;
        private WorldRenderer renderer;
        private CameraInfo info;

        public RenderingSimulator()
        {
            deferred = new DeferredRenderer(TW.Game);
            renderer = new WorldRenderer(TW.Model, deferred);

            var light = deferred.CreateDirectionalLight();
            light.LightDirection = Vector3.Normalize(new Vector3(1, -1, 1));
            light.ShadowsEnabled = true;


            info = TW.Model.GetSingleton<CameraInfo>();

        }

        public void Simulate()
        {
            TW.Game.Camera = info.ActiveCamera;
            renderer.ProcessWorldChanges();
            deferred.Draw();

        }
    }
}
