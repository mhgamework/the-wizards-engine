﻿using System;
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

        public override void OnCreated(IVoxelHandle handle)
        {
            townCenterService.CreateTown(handle.GetInternalVoxel());
        }
        public override void OnDestroyed(IVoxelHandle handle)
        {
            townCenterService.DestroyTown(townCenterService.GetTownForVoxel(handle.GetInternalVoxel()));
        }

        public override void Tick(IVoxelHandle handle)
        {
            handle.EachRandomInterval(1f, () => workersService.UpdateWorkerDistribution(townCenterService.GetTownForVoxel(handle.GetInternalVoxel())));

        }
    }
}