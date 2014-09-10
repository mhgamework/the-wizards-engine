using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using SlimDX;
using SlimDX.DirectInput;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;

namespace MHGameWork.TheWizards.GodGame
{
    public class ChangeHeightTool : IPlayerTool
    {
        private Dictionary<PlayerState, ChangeHeightToolPerPlayer> tools =
            new Dictionary<PlayerState, ChangeHeightToolPerPlayer>();

        private Internal.Model.World world;

        public ChangeHeightTool(Internal.Model.World world)
        {
            this.world = world;
        }

        public string Name { get { return "ChangeHeight"; } }
        public void OnLeftClick(PlayerState player, IVoxelHandle voxel)
        {
            getTool(player).OnLeftClick(voxel);
        }

        public void OnRightClick(PlayerState player, IVoxelHandle voxel)
        {
            getTool(player).OnRightClick(voxel);
        }

        public void OnKeypress(PlayerState player, IVoxelHandle voxel, Key key)
        {
            getTool(player).OnKeypress(voxel, key);
        }

        private ChangeHeightToolPerPlayer getTool(PlayerState player)
        {
            return tools.GetOrCreate(player, () => new ChangeHeightToolPerPlayer(world, player));
        }
    }
    public class ChangeHeightToolPerPlayer
    {
        private Internal.Model.World world;
        private readonly PlayerState player;
        public int Size { get { return player.HeightToolSize; } set { player.HeightToolSize = value; } }
        public HeightToolState State { get { return player.HeightToolState; } set { player.HeightToolState = value; } }

        private float flattenHeight;

        public enum HeightToolState
        {
            DEFAULT, SMOOTH, FLATTEN
        }

        public ChangeHeightToolPerPlayer(Internal.Model.World world, PlayerState player)
        {
            this.world = world;
            this.player = player;
            State = HeightToolState.DEFAULT;
        }

        public void OnLeftClick(IVoxelHandle voxel)
        {
            ProcessClick(voxel, true);

        }

        public void OnRightClick(IVoxelHandle voxel)
        {
            ProcessClick(voxel, false);
        }

        public void OnKeypress(IVoxelHandle voxel, Key key)
        {
            if (key == Key.NumberPadPlus)
                Size++;
            if (key == Key.NumberPadMinus)
                Size--;
            if (Size < 0)
                Size = 0;

            if (key == Key.NumberPadStar)
            {
                State = State == HeightToolState.SMOOTH ? HeightToolState.DEFAULT : HeightToolState.SMOOTH;
            }
            if (key == Key.NumberPadSlash)
            {
                State = State == HeightToolState.FLATTEN ? HeightToolState.DEFAULT : HeightToolState.FLATTEN;
            }
        }

        private void ProcessClick(IVoxelHandle voxel, bool isLeftClick)
        {
            switch (State)
            {
                case HeightToolState.DEFAULT:
                    DoDefault(voxel, isLeftClick);
                    break;
                case HeightToolState.SMOOTH:
                    DoSmooth(voxel, isLeftClick);
                    break;
                case HeightToolState.FLATTEN:
                    DoFlatten(voxel, isLeftClick);
                    break;
            }
        }

        private void DoDefault(IVoxelHandle voxel, bool isLeftClick)
        {
            var change = isLeftClick ? 1 : -1;
            foreach (var v in GetVoxelsInRange(voxel))
            {
                v.Data.Height += change;
                CheckHeight(v);
                world.NotifyVoxelChanged(v.GetInternalVoxel());
            }
        }

        private void DoSmooth(IVoxelHandle voxel, bool isLeftClick)
        {
            var change = isLeftClick ? 1 : -1;
            var voxels = GetVoxelsInRange(voxel);
            var averageHeight = Math.Floor(voxels.Sum(e => e.Data.Height) / voxels.Count);

            foreach (var v in voxels)
            {
                if (v.Data.Height > averageHeight)
                    v.Data.Height -= change;
                else if (v.Data.Height < averageHeight)
                    v.Data.Height += change;

                CheckHeight(v);
                world.NotifyVoxelChanged(v.GetInternalVoxel());
            }
        }

        private void DoFlatten(IVoxelHandle voxel, bool isLeftClick)
        {
            if (!isLeftClick)
            {
                flattenHeight = voxel.Data.Height;
                return;
            }

            var voxels = GetVoxelsInRange(voxel);
            foreach (var v in voxels)
            {
                v.Data.Height = flattenHeight;
                world.NotifyVoxelChanged(v.GetInternalVoxel());
            }
        }

        private void CheckHeight(IVoxelHandle voxel)
        {
            if (voxel.Data.Height < 0)
                voxel.Data.Height = 0;
        }

        private List<IVoxelHandle> GetVoxelsInRange(IVoxelHandle centerVoxel)
        {
            return world.GetRange(centerVoxel.GetInternalVoxel(), Size).Select(e => new IVoxelHandle(e)).ToList();
        }
    }
}
