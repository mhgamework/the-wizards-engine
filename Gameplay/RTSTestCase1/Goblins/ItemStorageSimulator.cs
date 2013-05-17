using MHGameWork.TheWizards.Engine;
using System.Linq;
using MHGameWork.TheWizards.RTSTestCase1.Goblins.Components;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins
{
    /// <summary>
    /// Ensures visibility of items in item storages
    /// </summary>
    public class ItemStorageSimulator:ISimulator
    {
        public void Simulate()
        {
            foreach (var i in TW.Data.Objects.OfType<IItemStorage>())
            {
                i.ItemStorage.UpdateItemLocations();
            }
        }
    }
}