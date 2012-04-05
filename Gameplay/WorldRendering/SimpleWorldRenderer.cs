using MHGameWork.TheWizards._XNA.World.Rendering;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX;

namespace MHGameWork.TheWizards.WorldRendering
{
    public class SimpleWorldRenderer : ISimulator
    {
        private DeferredRenderer deferred;
        private WorldRenderer renderer;
        private CameraInfo info;

        public SimpleWorldRenderer()
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
