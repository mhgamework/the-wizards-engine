using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Gameplay.Scripts
{
    public class RotateOnTheSpotScript : IWorldScript
    {
        private readonly ISimulationEngine engine;
        private IWorldObject obj;

        public RotateOnTheSpotScript(ISimulationEngine engine)
        {
            this.engine = engine;
        }

        public void Initialize(IWorldObject obj)
        {
            this.obj = obj;
        }

        public void Update()
        {
            obj.Rotation = Quaternion.RotationAxis(Vector3.UnitY, engine.CurrentTime);
        }
    }
}