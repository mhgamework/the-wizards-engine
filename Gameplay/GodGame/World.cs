using System;
using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using SlimDX;
using MHGameWork.TheWizards.Scattered._Engine;

namespace MHGameWork.TheWizards.GodGame
{
    /// <summary>
    /// World starts at 0,0 and grows in positive direction
    /// </summary>
    public class World
    {
        public Vector2 VoxelSize { get; private set; }
        private Array2D<GameVoxel> voxels;
        public World(int size, float voxelSize)
        {
            this.VoxelSize = new Vector2(voxelSize);
            voxels = new Array2D<GameVoxel>(new Point2(size, size));
            voxels.ForEach((v, p) => voxels[p] = new GameVoxel(this, p));
        }

        public GameVoxel GetVoxelAtGroundPos(Vector3 groundPos)
        {
            var index = Vector2.Modulate(groundPos.TakeXZ(), new Vector2(1 / VoxelSize.X, 1 / VoxelSize.Y));
            return voxels[index.Floor()];
        }

        public void ForEach(Action<GameVoxel, Point2> func)
        {
            voxels.ForEach(func);
        }

        public BoundingBox GetBoundingBox(Point2 gameVoxel)
        {
            var botLeft = Vector2.Modulate(gameVoxel, VoxelSize).ToXZ(0);
            return new BoundingBox(botLeft, botLeft + VoxelSize.ToXZ(0.1f));
        }

        public GameVoxel GetVoxel(Point2 p)
        {
            return voxels[p];
        }
    }
}