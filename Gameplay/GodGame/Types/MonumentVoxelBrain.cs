using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.GodGame.VoxelInfoVisualizers;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class MonumentVoxelBrain
    {
        private readonly InfestationVoxelType infestationVoxelType;
        private readonly UserInputService inputService;
        private ItemType crystalType;
        private float nextTick;

        public IVoxelHandle Handle { get; private set; }

        public MonumentVoxelBrain(ItemTypesFactory typesFactory, UserInputService inputService, InfestationVoxelType infestationVoxelType, IVoxelHandle handle)
        {
            this.inputService = inputService;
            this.infestationVoxelType = infestationVoxelType;
            crystalType = typesFactory.CrystalType;
            this.Handle = handle;

        }


        public void Tick()
        {
            if (nextTick > Handle.TotalTime) return;
            nextTick = Handle.TotalTime + 0.5f;

            tryInfuse();


        }

        public void ApplyMonumentRangeInput(int value)
        {
            Handle.Data.DataValue = value;
        }

        /// <summary>
        /// NOTE: infestation works soemwhat wierdly (the magic levels are only applied to infested tiles)
        /// </summary>
        /// <param name="handle"></param>
        private void tryInfuse()
        {
            if (!hasAccessToCrystal()) return;

            foreach (var v in Handle.GetRange(getRadius()).Where(v => Handle.DistanceTo(v) <= getRadius()).OrderBy(Handle.DistanceTo))
            {
                if (v.Type != infestationVoxelType || v.Data.MagicLevel > 2)
                    continue;

                takeAndConsumeCrystal();
                infestationVoxelType.CureInfestation(v);
                return;
            }
        }

        private int getRadius()
        {
            return Handle.Data.DataValue;
        }

        private void takeAndConsumeCrystal()
        {
            if (!hasAccessToCrystal()) throw new InvalidOperationException();

            var warehouse = getAdjacentWarehousesWithCrystals().First();
            warehouse.Data.Inventory.DestroyItems(getCrystalType(), 1);
        }



        private bool hasAccessToCrystal()
        {
            return getAdjacentWarehousesWithCrystals().Any();
        }

        private IEnumerable<IVoxelHandle> getAdjacentWarehousesWithCrystals()
        {
            return Handle.Get4Connected().Where(w => w.Data.Inventory[getCrystalType()] > 0);
        }

        private ItemType getCrystalType()
        {
            return crystalType;
        }

        public IEnumerable<IRenderable> GetInfoVisualizers()
        {
            yield return new RangeVisualizer(Handle, getRadius());

        }

        public IEnumerable<IRenderable> GetCustomVisualizers()
        {
            yield return new ValueControlVisualizer(Handle, () => Handle.Data.DataValue, v => inputService.SendMonumentRangeInput(Handle, v),
                                                    Matrix.Scaling(0.3f, 0.3f, 0.3f) * Matrix.Translation(0, 2, 0)).Alter(v => v.ValueControl.MaxValue = 50);
        }

        public void Dispose()
        {
        }
    }
}