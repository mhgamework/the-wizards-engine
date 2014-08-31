using System;
using System.Collections.Generic;
using System.Drawing;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.VoxelInfoVisualizers;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Model;
using System.Linq;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class MonumentType : GameVoxelType
    {
        public MonumentType()
            : base("Monument")
        {
            Color = Color.Green;

        }

        private float prevFrameTime;
        private float nextTick;

        private bool isTickFrame = false;

        public override void Tick(IVoxelHandle handle)
        {
            if (prevFrameTime != handle.TotalTime)
            {
                //This is the first tick on a road this frame
                if (nextTick < handle.TotalTime)
                {
                    isTickFrame = true;
                    nextTick = handle.TotalTime + 0.1f;
                }
                else
                {
                    isTickFrame = false;
                }

                prevFrameTime = handle.TotalTime;

            }
            if (!isTickFrame) return;

            tryInfuse(handle);


        }

        /// <summary>
        /// NOTE: infestation works soemwhat wierdly (the magic levels are only applied to infested tiles)
        /// </summary>
        /// <param name="handle"></param>
        private void tryInfuse(IVoxelHandle handle)
        {
            if (!hasAccessToCrystal(handle)) return;

            foreach (var v in handle.GetRange(getRadius(handle)).Where(v => handle.DistanceTo(v) <= getRadius(handle)).OrderBy(handle.DistanceTo))
            {
                if (v.Type != Infestation || v.Data.MagicLevel > 2)
                    continue;

                takeAndConsumeCrystal(handle);
                Infestation.CureInfestation(v);
                return;
            }
        }

        private int getRadius(IVoxelHandle handle)
        {
            return handle.Data.DataValue;
        }

        private void takeAndConsumeCrystal(IVoxelHandle voxelHandle)
        {
            if (!hasAccessToCrystal(voxelHandle)) throw new InvalidOperationException();

            var warehouse = getAdjacentWarehousesWithCrystals(voxelHandle).First();
            warehouse.Data.Inventory.DestroyItems(getCrystalType(), 1);
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

        public override IEnumerable<IRenderable> GetInfoVisualizers(IVoxelHandle handle)
        {
            foreach (var v in base.GetInfoVisualizers(handle))
                yield return v;
            yield return new RangeVisualizer(handle, getRadius(handle));

        }

        public override bool DontShowDataValue { get { return true; } }

        public override IEnumerable<IRenderable> GetCustomVisualizers(IVoxelHandle handle)
        {
            yield return new ValueControlVisualizer(handle, () => handle.Data.DataValue, v => handle.Data.DataValue = v,
                  Matrix.Scaling(0.3f, 0.3f, 0.3f) * Matrix.Translation(0, 2, 0)).Alter(v => v.ValueControl.MaxValue = 50);
        }
    }
}