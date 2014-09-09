﻿using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    public class CreateLandTool : IPlayerTool
    {
        private readonly Internal.Model.World world;

        public CreateLandTool(Internal.Model.World world)
        {
            this.world = world;
        }

        public string Name { get { return "CreateLand"; } }

        public void OnLeftClick(IVoxelHandle voxel)
        {
            voxel.ChangeType(GameVoxelType.Air);
        }
        public void OnRightClick(IVoxelHandle voxel)
        {
            voxel.ChangeType(GameVoxelType.Land);
        }

        public void OnKeypress(IVoxelHandle voxel, Key key)
        {
            
        }

        public override string ToString()
        {
            return "Handler: " + Name;
        }

    }
}