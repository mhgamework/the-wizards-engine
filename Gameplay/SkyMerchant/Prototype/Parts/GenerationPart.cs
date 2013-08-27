namespace MHGameWork.TheWizards.SkyMerchant.Prototype.Parts
{
    public class GenerationPart
    {

        #region Injection

        public IItemFactory Factory { get; set; }
        public ISimulationEngine SimulationEngine { get; set; }

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