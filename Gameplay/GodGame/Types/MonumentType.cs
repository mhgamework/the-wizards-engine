using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using System.Reflection;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.GodGame.UI;
using MHGameWork.TheWizards.GodGame.VoxelInfoVisualizers;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Model;
using System.Linq;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class MonumentType : GameVoxelType
    {
        private readonly Func<IVoxelHandle, MonumentVoxel> createMonumentVoxel;
        private readonly Internal.Model.World world;

        public MonumentType(Func<IVoxelHandle, MonumentVoxel> createMonumentVoxel, Internal.Model.World world)
            : base("Monument")
        {
            this.createMonumentVoxel = createMonumentVoxel;
            this.world = world;
            Color = Color.Green;

        }

        private List<MonumentVoxel> monuments = new List<MonumentVoxel>();

        public override void PerFrameTick()
        {
            updateMonumentsList();
            monuments.ForEach(m => m.Tick());
        }

        public override IEnumerable<IRenderable> GetCustomVisualizers(IVoxelHandle handle)
        {
            return getMonument(handle.GetInternalVoxel()).GetCustomVisualizers();
        }

        private void updateMonumentsList()
        {
            var added = world.ChangedVoxels.Where(v => v.Type == this && getMonument(v) == null);
            var removed = world.ChangedVoxels.Where(v => v.Type != this && getMonument(v) != null);

            removed.Select(getMonument).ToArray()
                .ForEach(m =>
                {
                    m.Dispose();
                    monuments.Remove(m);
                });

            added.ForEach(v =>
                {
                    var m = createMonumentVoxel(new IVoxelHandle(v));
                    monuments.Add(m);

                });
        }

        private MonumentVoxel getMonument(GameVoxel v)
        {
            return monuments.FirstOrDefault(m => v == m.Handle.GetInternalVoxel());
        }


        public override IEnumerable<IRenderable> GetInfoVisualizers(IVoxelHandle handle)
        {
            return getMonument(handle).GetInfoVisualizers();
        }


        public void ApplyMonumentRangeInput(IVoxelHandle voxelHandle, int value)
        {
            getMonument(voxelHandle).ApplyMonumentRangeInput(value);
        }

        public class MonumentVoxel
        {
            private readonly InfestationVoxelType infestationVoxelType;
            private readonly UserInputService inputService;
            private ItemType crystalType;
            private float nextTick;

            public IVoxelHandle Handle { get; private set; }

            public MonumentVoxel(ItemTypesFactory typesFactory, UserInputService inputService, InfestationVoxelType infestationVoxelType, IVoxelHandle handle)
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
                Handle = new IVoxelHandle(Handle.GetInternalVoxel()); // copy
                yield return new ValueControlVisualizer(Handle, () => Handle.Data.DataValue, v => inputService.SendMonumentRangeInput(Handle, v),
                      Matrix.Scaling(0.3f, 0.3f, 0.3f) * Matrix.Translation(0, 2, 0)).Alter(v => v.ValueControl.MaxValue = 50);
            }

            public void Dispose()
            {
            }
        }

        
    }
}