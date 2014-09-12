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

        public float FlattenHeight { get { return player.HeightToolFlattenHeight; } set { player.HeightToolFlattenHeight = value; } }
        public float StandardDeviation { get { return player.HeightToolStandardDeviation; } set { player.HeightToolStandardDeviation = value; } }
        public float Amplitude { get { return player.HeightToolAmplitude; } set { player.HeightToolAmplitude = value; } }

        public enum HeightToolState
        {
            DEFAULT, SMOOTH, FLATTEN
        }

        public ChangeHeightToolPerPlayer(Internal.Model.World world, PlayerState player)
        {
            this.world = world;
            this.player = player;
            State = HeightToolState.DEFAULT;
            StandardDeviation = 2;
            Amplitude = 1;
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

            if (key == Key.NumberPad8)
                Amplitude++;
            if (key == Key.NumberPad2)
                Amplitude--;
            if (Amplitude < 1)
                Amplitude = 1;

            if (key == Key.NumberPad4)
                StandardDeviation--;
            if (key == Key.NumberPad6)
                StandardDeviation++;
            if (StandardDeviation < 1)
                StandardDeviation = 1;

            if (key == Key.NumberPadStar)
            {
                State = State == HeightToolState.SMOOTH ? HeightToolState.DEFAULT : HeightToolState.SMOOTH;
            }
            if (key == Key.NumberPadSlash)
            {
                State = State == HeightToolState.FLATTEN ? HeightToolState.DEFAULT : HeightToolState.FLATTEN;
            }

            if (key == Key.NumberPadEnter)
                normalizeWorld();
        }

        private void ProcessClick(IVoxelHandle voxel, bool isLeftClick)
        {
            switch (State)
            {
                case HeightToolState.DEFAULT:
                    DoDefault(voxel, isLeftClick);
                    break;
                case HeightToolState.SMOOTH:
                        DoSmooth2(voxel, isLeftClick);
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
                v.Data.Height += getHeightGaussian(xDiff(voxel, v), yDiff(voxel, v), Amplitude * change, StandardDeviation);
                CheckHeight(v);
                world.NotifyVoxelChanged(v.GetInternalVoxel());
            }
        }

        private void DoSmooth2(IVoxelHandle voxel, bool isLeftClick)
        {
            var change = isLeftClick ? 1 : -1;
            var voxels = GetVoxelsInRange(voxel);
            var averageHeight = voxels.Sum(e => e.Data.Height) / (float)voxels.Count;

            // using 1/4 of a 3x3 kernel
            float[][] matrix = {
                            new []{ 1/4f, 1/8f}, 
                            new []{ 1/8f, 1/16f}};

            foreach (var v in voxels)
            {
                var weightedSum = 0f;
                foreach (var n in v.Get8Connected())
                {
                    weightedSum += matrix[(int)xDiff(n, v)][(int)yDiff(n, v)] * n.Data.Height;
                }
                weightedSum += matrix[0][0] * v.Data.Height;
                v.Data.Height = weightedSum;
                /*if (v.Data.Height > averageHeight)
                    v.Data.Height = (float)Math.Floor(v.Data.Height - getHeightGaussian(xDiff(voxel, v), yDiff(voxel, v), Amplitude * change, StandardDeviation));
                else if (v.Data.Height < averageHeight)
                    v.Data.Height = (float)Math.Floor(v.Data.Height + getHeightGaussian(xDiff(voxel, v), yDiff(voxel, v), Amplitude * change, StandardDeviation));*/

                CheckHeight(v);
            }
        }
        private void DoSmooth(IVoxelHandle voxel, bool isLeftClick)
        {
            var change = isLeftClick ? 1 : -1;
            var voxels = GetVoxelsInRange(voxel);
            var averageHeight = voxels.Sum(e => e.Data.Height) / (float)voxels.Count;

            foreach (var v in voxels)
            {
                if (v.Data.Height > averageHeight)
                    v.Data.Height = (float)Math.Floor(v.Data.Height - getHeightGaussian(xDiff(voxel, v), yDiff(voxel, v), Amplitude * change, StandardDeviation));
                else if (v.Data.Height < averageHeight)
                    v.Data.Height = (float)Math.Floor(v.Data.Height + getHeightGaussian(xDiff(voxel, v), yDiff(voxel, v), Amplitude * change, StandardDeviation));

                CheckHeight(v);
                world.NotifyVoxelChanged(v.GetInternalVoxel());
            }
        }

        private float xDiff(IVoxelHandle h1, IVoxelHandle h2)
        {
            return Math.Abs(h1.GetInternalVoxel().Coord.X - h2.GetInternalVoxel().Coord.X);
        }

        private float yDiff(IVoxelHandle h1, IVoxelHandle h2)
        {
            return Math.Abs(h1.GetInternalVoxel().Coord.Y - h2.GetInternalVoxel().Coord.Y);
        }

        private float getHeightGaussian(float xDiff, float yDiff, float amplitude, float standardDev)
        {
            return (float)(amplitude * Math.Exp(-(xDiff * xDiff + yDiff * yDiff) / (2 * standardDev * standardDev)));
        }

        private void normalizeWorld()
        {
            world.ForEach((voxel, _) => voxel.Data.Height = (float)Math.Floor(voxel.Data.Height));
        }

        private void DoFlatten(IVoxelHandle voxel, bool isLeftClick)
        {
            if (!isLeftClick)
            {
                FlattenHeight = voxel.Data.Height;
                return;
            }

            var voxels = GetVoxelsInRange(voxel);
            foreach (var v in voxels)
            {
                v.Data.Height = FlattenHeight;
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
