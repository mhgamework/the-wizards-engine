using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame
{
    public class VoxelData
    {
        public VoxelData()
        {
            Inventory = new Inventory();
            Road = RoadType.RoadData.Empty;

        }

        public int DataValue { get; set; }
        public int MagicLevel { get; set; }

        /// <summary>
        /// TODO: could be optimized to not always store an inventory
        /// </summary>
        public Inventory Inventory { get; private set; }

        public RoadType.RoadData Road { get; private set; }

       
    }
}