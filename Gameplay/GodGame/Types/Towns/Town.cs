using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Types.Workers;

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

        public Town(TownCenterService service)
        {
            this.service = service;
        }

        // Data
        public HashSet<IVoxel> TownVoxels = new HashSet<IVoxel>();


        // Derived
        public HashSet<House> Houses { get { return service.GetHouses(this); } }

        public bool IsAtBorder(IVoxel voxel)
        {
            if (TownVoxels.Contains(voxel)) return false;
            if (!TownVoxels.Overlaps(voxel.Get4Connected())) return false;
            return true;
        }

        public IEnumerable<WorkerProducer> Producers { get { throw new NotImplementedException(); } }
        public IEnumerable<WorkerConsumer> Consumers { get { throw new NotImplementedException(); } }
    }
}