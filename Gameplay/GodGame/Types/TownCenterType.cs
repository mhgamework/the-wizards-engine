using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.GodGame.Types.Towns;
using MHGameWork.TheWizards.GodGame.Types.Towns.Workers;
using MHGameWork.TheWizards.GodGame.VoxelInfoVisualizers;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Types
{
    /// <summary>
    /// A town center distributes workers within its boundary. 
    /// Workers are provided from houses to industry buildings
    /// </summary>
    public class TownCenterType : GameVoxelType
    {
        private readonly TownCenterService townCenterService;
        private readonly WorkersService workersService;

        public TownCenterType(TownCenterService townCenterService, WorkersService workersService)
            : base("TownCenter")
        {
            this.townCenterService = townCenterService;
            this.workersService = workersService;
            ReceiveCreationEvents = true;


        }

        public override void OnCreated(IVoxel handle)
        {
            townCenterService.CreateTown(handle);
        }
        public override void OnDestroyed(IVoxel handle)
        {
            townCenterService.DestroyTown(townCenterService.GetTownForVoxel(handle));
        }

        private float nextUpdate = 0;
        public override void PerFrameTick()
        {
            if (TW.Graphics.TotalRunTime < nextUpdate) return;
            nextUpdate = TW.Graphics.TotalRunTime + 1;
            townCenterService.Towns.ForEach(t => workersService.UpdateWorkerDistribution(t));
        }

        protected override string getDebugDescription(IVoxelHandle handle)
        {
            Town town = townCenterService.GetTownForVoxel((IVoxel) handle);
            var totalWorkers = town.Producers.Sum(p => p.ProvidedWorkersAmount);
            var assignedWorkers = town.Consumers.Sum(p => p.AllocatedWorkersCount);
            return string.Format("Workers: {0:000} - Assigned: {1:000}\nNextUpdate: {2}",
                                 totalWorkers,
                                 assignedWorkers,
                                 (nextUpdate - TW.Graphics.TotalRunTime).ToString("00.00"));
        }
    }
}