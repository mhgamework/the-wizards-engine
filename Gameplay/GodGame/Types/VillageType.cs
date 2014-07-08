using System.Drawing;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class VillageType : GameVoxelType
    {
        public VillageType()
            : base("Village")
        {
        }



        public override void Tick(IVoxelHandle handle)
        {
            handle.EachRandomInterval(1, () => doWork(handle));

        }

        private void doWork(IVoxelHandle handle)
        {
            var warehouse =
                handle.GetRange(5).FirstOrDefault(v => v.Type == Warehouse && v.Data.DataValue < 20);
            if (warehouse == null) return;

            var forest = handle.GetRange(5).FirstOrDefault(v => v.Type == Forest && v.Data.DataValue > 0);
            if (forest == null) return;

            forest.Data.DataValue--;
            warehouse.Data.DataValue++;
        }
    }
}