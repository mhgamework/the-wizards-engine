using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.Parts
{
    /// <summary>
    /// Currently the trader generates resources per second, 
    /// later on it should acquire them from other computer driven robot players
    /// </summary>
    [ModelObjectChanged]
    public class TraderPart : EngineModelObject
    {
        #region Injection
        public IItemFactory ItemFactory { get; set; }
        public ISimulationEngine SimulationEngine { get; set; }
        #endregion

        public TraderPart()
        {

        }

        public ItemType WantsType { get; set; }
        public int WantsAmount { get; set; }

        public ItemType GivesType { get; set; }
        public int GivesAmount { get; set; }

        public float StoredResourcesCount { get; set; }

        public void PerformTrade(RobotPlayerPart player)
        {
            if (!CanTradeWith(player)) return;

            var ret = player.TakeItems(WantsType, WantsAmount);
            foreach (var i in ret)
            {
                ItemFactory.Destroy(i);
            }

            for (int i = 0; i < GivesAmount; i++)
            {
                player.Pickup(ItemFactory.CreateItem(GivesType));
            }

            StoredResourcesCount -= GivesAmount;


        }

        public bool CanTradeWith(RobotPlayerPart player)
        {
            return player.HasItems(WantsType, WantsAmount) && player.CanPickup(GivesType, GivesAmount) && StoredResourcesCount >= GivesAmount;
        }


        public void SimulateResourcesGeneration()
        {
            StoredResourcesCount += SimulationEngine.Elapsed;
        }

        public interface IItemFactory
        {
            ItemPart CreateItem(ItemType type);
            void Destroy(ItemPart item);
        }
    }
}