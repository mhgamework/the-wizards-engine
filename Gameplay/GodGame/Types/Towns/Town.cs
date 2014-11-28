using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Types.Towns.Workers;

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
            TownVoxels = datastoreRecord.GetSet<IVoxel>("TownVoxels");
            datastoreRecord.Bind(this);
        }

        // Data
        public HashSet<IVoxel> TownVoxels = new HashSet<IVoxel>();


        /// <summary>
        /// Returns true when given voxel borders to the town (so not in the town
        /// </summary>
        public bool IsAtBorder(IVoxel voxel)
        {
            if (TownVoxels.Contains(voxel)) return false;
            if (!TownVoxels.Overlaps(voxel.Get4Connected())) return false;
            return true;
        }

        public IEnumerable<IWorkerProducer> Producers { get { return service.GetProducers(this); } }
        public IEnumerable<IWorkerConsumer> Consumers { get { return service.GetConsumers(this); } }

        public bool CanRemove(IVoxel getInternalVoxel)
        {
            if (TownVoxels.Count == 1) return false;
            var setWithout = new HashSet<IVoxel>(TownVoxels.Where(v => v != getInternalVoxel));
            var removedSet = new HashSet<IVoxel>(TownVoxels.Where(v => v != getInternalVoxel));

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
    }
}