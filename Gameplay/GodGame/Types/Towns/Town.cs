using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Types.Towns.Workers;
using MHGameWork.TheWizards.SkyMerchant._Tests.Ideas;

namespace MHGameWork.TheWizards.GodGame.Types.Towns
{
    /// <summary>
    /// Represents a town
    /// A town has a boundary, all voxels inside the boundary belong to this town only
    ///     Currently simply storing town voxels
    /// 
    /// A town can contain houses and 'workerclients'
    /// Houses provide workers
    /// Workerclients use workers
    /// 
    /// 
    /// IDEA: add the [InvertDatastorageControl] attribute, which indicates that this control releases control over its field storage
    ///     This is equivalent to adding an additional constructor parameter, with an interface that gets/sets the field data
    ///     This can be done Using AOP, and we can still configure what to inject at the container level
    ///     Use this to allow for persistence
    /// </summary>
    public class Town : Workers.ITown
    {
        private readonly TownCenterService service;
        private readonly GenericDatastoreRecord datastoreRecord;

        public Town(TownCenterService service, GenericDatastoreRecord datastoreRecord)
        {
            this.service = service;
            this.datastoreRecord = datastoreRecord;
            townVoxels = datastoreRecord.GetSet<IVoxel>("TownVoxels");
            datastoreRecord.Bind(this);
        }

        // Data
        private HashSet<IVoxel> townVoxels = new HashSet<IVoxel>();
        public IEnumerable<IVoxel> TownVoxels { get { return townVoxels; } }
        public IVoxel TownCenter
        {
            get { return datastoreRecord.Get<IVoxel>("TownCenter"); }
            set
            {
                datastoreRecord.Set<IVoxel>("TownCenter", value);
            }
        }


        /// <summary>
        /// Returns true when given voxel borders to the town (so not in the town)
        /// </summary>
        public bool IsAtBorder(IVoxel voxel)
        {
            if (townVoxels.Contains(voxel)) return false;
            if (!townVoxels.Overlaps(voxel.Get4Connected())) return false;
            return true;
        }

        public IEnumerable<IWorkerProducer> Producers { get { return service.GetProducers(this); } }
        public IEnumerable<IWorkerConsumer> Consumers { get { return service.GetConsumers(this); } }

        public void AddVoxel(IVoxel voxel)
        {
            if (!CanAddVoxel(voxel)) throw new InvalidOperationException("Invalid voxel to add");
            townVoxels.Add(voxel);

            ((IVoxelHandle)voxel).MarkChanged();
            ((IVoxelHandle)voxel).Get8Connected().ForEach(v => v.MarkChanged());
        }

        public bool CanAddVoxel(IVoxel voxel)
        {
            if (townVoxels.Contains(voxel)) return false;
            if (!IsAtBorder(voxel)) return false;
            if (service.GetTownForVoxel(voxel) != null) return false;
            return true;
        }

        public void RemoveVoxel(IVoxel voxel)
        {
            if (!CanRemove(voxel)) throw new InvalidOperationException();
            townVoxels.Remove(voxel);

            ((IVoxelHandle)voxel).MarkChanged();
            ((IVoxelHandle)voxel).Get8Connected().ForEach(v => v.MarkChanged());
        }

        public bool CanRemove(IVoxel getInternalVoxel)
        {
            if (townVoxels.Count == 1) return false;
            var setWithout = new HashSet<IVoxel>(townVoxels.Where(v => v != getInternalVoxel));
            var removedSet = new HashSet<IVoxel>(townVoxels.Where(v => v != getInternalVoxel));

            // Note i think this is simplye the VisitOnce function in the Utilities dll on my work pc
            var queue = new Queue<IVoxel>();
            queue.Enqueue(removedSet.First());
            while (queue.Count > 0)
            {
                var curr = queue.Dequeue();
                if (!removedSet.Contains(curr)) continue;
                removedSet.Remove(curr);
                curr.Get4Connected().Intersect(setWithout).ForEach(queue.Enqueue);
            }
            return removedSet.Count == 0; // If the removedset is empty then all voxels are still conected when given voxel is removed.

        }

        public void SetTownCenter(IVoxel center)
        {
            if (TownCenter != null) throw new InvalidOperationException("Changing town center is not supported");
            TownCenter = center;
            townVoxels.Add(center);
            ((IVoxelHandle)center).MarkChanged();
            ((IVoxelHandle)center).Get8Connected().ForEach(v => v.MarkChanged());
        }
    }
}