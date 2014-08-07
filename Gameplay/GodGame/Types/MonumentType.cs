using System;
using System.Collections.Generic;
using System.Drawing;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class MonumentType : GameVoxelType
    {
        private int radius = 7;

        public MonumentType()
            : base("Monument")
        {
            Color = Color.Green;

        }


        public override void Tick(IVoxelHandle handle)
        {
            var nextInfuse = handle.Data.DataValue / 1000;

            if (nextInfuse > handle.TotalTime) return;
            handle.Data.DataValue = (int)((handle.TotalTime + 1) * 1000);

            tryInfuse(handle);
        }

        /// <summary>
        /// NOTE: infestation works soemwhat wierdly (the magic levels are only applied to infested tiles)
        /// </summary>
        /// <param name="handle"></param>
        private void tryInfuse(IVoxelHandle handle)
        {
            if (!hasAccessToCrystal(handle)) return;

            foreach (var v in handle.GetRange(radius).Where(v => handle.DistanceTo(v) <= 7).OrderBy(handle.DistanceTo))
            {
                if (v.Type != Infestation || v.Data.MagicLevel > 2)
                    continue;

                takeAndConsumeCrystal(handle);
                cureInfestation(v);
                return;
            }
        }

        private void takeAndConsumeCrystal(IVoxelHandle voxelHandle)
        {
            if (!hasAccessToCrystal(voxelHandle)) throw new InvalidOperationException();

            var warehouse = getAdjacentWarehousesWithCrystals(voxelHandle).First();
            warehouse.Data.Inventory.DestroyItems(getCrystalType(), 1);
        }

        private static void cureInfestation(IVoxelHandle v)
        {
            if (v.Type != Infestation) throw new InvalidOperationException();

            if (v.Seeder.NextFloat(0, 1) < 0.2f)
            {
                v.ChangeType(Ore);
                v.Data.DataValue = 20;
            }
            else
                v.ChangeType(Land);

            v.Data.MagicLevel = 10;

        }

        private bool hasAccessToCrystal(IVoxelHandle handle)
        {
            return getAdjacentWarehousesWithCrystals(handle).Any();
        }

        private IEnumerable<IVoxelHandle> getAdjacentWarehousesWithCrystals(IVoxelHandle handle)
        {
            return handle.Get4Connected().Where(w => w.Data.Inventory[getCrystalType()] > 0);
        }

        private static ItemType getCrystalType()
        {
            return Ore.GetOreItemType(null);
        }

        public override IEnumerable<IVoxelInfoVisualizer> GetInfoVisualizers(IVoxelHandle handle)
        {
            foreach (var v in base.GetInfoVisualizers(handle))
                yield return v;
            yield return new RangeVisualizer(handle, radius);

        }

        public override bool DontShowDataValue { get { return true; } }

    }
}