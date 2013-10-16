using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Gameplay.Scripts
{
    public class RotateOnTheSpotScript : IWorldScript
    {
        private readonly ISimulationEngine engine;
        private readonly IPositionComponent position;

        public RotateOnTheSpotScript(ISimulationEngine engine, IPositionComponent position)
        {
            this.engine = engine;
            this.position = position;
        }

        public void Update()
        {
            position.Rotation = Quaternion.RotationAxis(Vector3.UnitY, engine.CurrentTime);
        }
    }
}