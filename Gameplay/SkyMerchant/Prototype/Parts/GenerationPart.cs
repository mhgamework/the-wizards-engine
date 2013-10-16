using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.SkyMerchant._Engine.Windsor;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.Parts
{
    [ModelObjectChanged]
    public class GenerationPart : EngineModelObject
    {
        private ISimulationEngine simulationEngine;

        #region Injection
        [NonOptional]
        public IItemFactory Factory { get; set; }
        [NonOptional]
        public ISimulationEngine SimulationEngine
        {
            get { return simulationEngine; }
            set
            {
                simulationEngine = value;
                NextResourceGeneration = SimulationEngine.CurrentTime + GenerationInterval;
            }
        }

        #endregion

        public GenerationPart()
        {
            NextResourceGeneration = -1;
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
            NextResourceGeneration = SimulationEngine.CurrentTime + GenerationInterval;
        }

        public void SimulateResourceGeneration()
        {
            if (HasResource) return;
            if (SimulationEngine.CurrentTime < NextResourceGeneration) return;

            HasResource = true;

        }


        public interface IItemFactory
        {
            ItemPart CreateItem();
        }
    }
}