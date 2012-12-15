using MHGameWork.TheWizards.Physics;

namespace MHGameWork.TheWizards.Engine.PhysX
{
    /// <summary>
    /// Renders PhysX debug information
    /// </summary>
    public class PhysXDebugRendererSimulator : ISimulator
    {
        private PhysicsDebugRenderer debugRenderer;

        public PhysXDebugRendererSimulator()
        {
            debugRenderer = new PhysicsDebugRenderer(TW.Graphics,TW.Physics.Scene);
            debugRenderer.Initialize();
        }

        public void Simulate()
        {
            debugRenderer.Render();
        }
    }
}
