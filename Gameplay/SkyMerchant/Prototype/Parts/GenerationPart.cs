using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.Parts
{
    [ModelObjectChanged]
    public class GenerationPart : EngineModelObject,IGameObjectComponent
    {
        private ISimulationEngine simulationEngine;

        public IItemFactory Factory { get; set; }

        public GenerationPart(ISimulationEngine simulationEngine)
        {
            this.simulationEngine = simulationEngine;
            NextResourceGeneration = simulationEngine.CurrentTime + GenerationInterval;
            GenerationInterval = 10;
        }

        public bool HasResource { get; set; }
        public float NextResourceGeneration { get; set; }
        public float GenerationInterval { get; set; }

        public void PlayerPickResource(RobotPlayerPart player)
        {
            if (!HasResource) return;

            var item = Factory.CreateItem();

            player.Pickup(item);

            HasResource = false;
            NextResourceGeneration = simulationEngine.CurrentTime + GenerationInterval;
        }

        public void SimulateResourceGeneration()
        {
            if (HasResource) return;
            if (simulationEngine.CurrentTime < NextResourceGeneration) return;

            HasResource = true;

        }


        public interface IItemFactory
        {
            ItemPart CreateItem();
        }
    }
}