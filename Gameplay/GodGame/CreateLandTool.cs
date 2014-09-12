﻿using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    public class CreateLandTool : IPlayerTool
    {
        private readonly Internal.Model.World world;
        private readonly AirType air;
        private readonly LandType land;

        public CreateLandTool(Internal.Model.World world, AirType air, LandType land)
        {
            this.world = world;
            this.air = air;
            this.land = land;
        }

        public string Name { get { return "CreateLand"; } }

        public void OnLeftClick(PlayerState player, IVoxelHandle voxel)
        {
            voxel.ChangeType(air);
        }
        public void OnRightClick(PlayerState player, IVoxelHandle voxel)
        {
            voxel.ChangeType(land);
        }

        public void OnKeypress(PlayerState player, IVoxelHandle voxel, Key key)
        {

        }

        public override string ToString()
        {
            return "Handler: " + Name;
        }

    }
}