using System;
using System.Collections.Generic;
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
        public int WorldSize { get { return voxels.Size.X; } }
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

        public IEnumerable<GameVoxel> Get8Connected(Point2 coord)
        {
            return voxels.Get8Connected(coord);
        }


        public IEnumerable<GameVoxel> GetRange(GameVoxel center, int radius)
        {
            for (int x = center.Coord.X - radius; x <= center.Coord.X + radius; x++)
                for (int y = center.Coord.Y - radius; y <= center.Coord.Y + radius; y++)
                    yield return voxels[new Point2(x, y)];
        }
    }
}