using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.PhysX;
using SlimDX;

namespace MHGameWork.TheWizards.Simulators
{
    /// <summary>
    /// Renders PhysX debug information
    /// </summary>
    public class PhysXDebugRendererSimulator : ISimulator
    {
        private PhysicsDebugRenderer debugRenderer;

        public PhysXDebugRendererSimulator()
        {
            debugRenderer = new PhysicsDebugRenderer(TW.Game,TW.Scene);
            debugRenderer.Initialize();
        }

        public void Simulate()
        {
            debugRenderer.Render();
        }
    }
}
