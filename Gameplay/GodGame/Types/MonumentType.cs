using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using System.Reflection;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.UI;
using MHGameWork.TheWizards.Rendering;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class MonumentType : GameVoxelType
    {
        private readonly Func<IVoxelHandle, MonumentVoxelBrain> createMonumentVoxel;
        private readonly Internal.Model.World world;

        public MonumentType(Func<IVoxelHandle, MonumentVoxelBrain> createMonumentVoxel, Internal.Model.World world)
            : base("Monument")
        {
            this.createMonumentVoxel = createMonumentVoxel;
            this.world = world;
            Color = Color.Green;

        }

        private List<MonumentVoxelBrain> monuments = new List<MonumentVoxelBrain>();

        public override void PerFrameTick()
        {
            updateMonumentsList();
            monuments.ForEach(m => m.Tick());
        }

        public override IEnumerable<IRenderable> GetCustomVisualizers(IVoxelHandle handle)
        {
            return getOrCreateMonument(handle.GetInternalVoxel()).GetCustomVisualizers();
        }

        private void updateMonumentsList()
        {
            return;
            var added = world.ChangedVoxels.Where(v => v.Type == this && getOrCreateMonument(v) == null);
            var removed = world.ChangedVoxels.Where(v => v.Type != this && getOrCreateMonument(v) != null);

            removed.Select(getOrCreateMonument).ToArray()
                .ForEach(m =>
                {
                    m.Dispose();
                    monuments.Remove(m);
                });

            added.ForEach(v =>
                {
                    getOrCreateMonument(v);//Should create if new

                });
        }

        private MonumentVoxelBrain getOrCreateMonument(IVoxelHandle v)
        {
            var ret = monuments.FirstOrDefault(m => v == m.Handle.GetInternalVoxel());
            if (ret == null)
            {
                ret = createMonumentVoxel(v);
                monuments.Add(ret);
            }
            return ret;
        }


        public override IEnumerable<IRenderable> GetInfoVisualizers(IVoxelHandle handle)
        {
            return getOrCreateMonument(handle).GetInfoVisualizers();
        }


        public void ApplyMonumentRangeInput(IVoxelHandle voxelHandle, int value)
        {
            getOrCreateMonument(voxelHandle).ApplyMonumentRangeInput(value);
        }
    }
}