﻿using System.Drawing;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class VillageType : GameVoxelType
    {
        public VillageType()
            : base("Village")
        {
        }



        public override void Tick(ITickHandle handle)
        {
            handle.EachRandomInterval(1, () => doWork(handle));

        }

        private void doWork(ITickHandle handle)
        {
            var warehouse =
                handle.GetRange(5).FirstOrDefault(v => v.Type == WarehouseType && v.DataValue < 20);
            if (warehouse == null) return;

            var forest = handle.GetRange(5).FirstOrDefault(v => v.Type == Forest && v.DataValue > 0);
            if (forest == null) return;

            forest.DataValue--;
            warehouse.DataValue++;
        }
    }
}