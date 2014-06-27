using System;
using DirectX11;
using MHGameWork.TheWizards.GodGame.Model;
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
        private Vector2 voxelSize;
        private Array2D<GameVoxel> voxels;
        public World(int size, float voxelSize)
        {
            this.voxelSize = new Vector2(voxelSize);
            voxels = new Array2D<GameVoxel>(new Point2(size, size));
            voxels.ForEach((v, p) => voxels[p] = new GameVoxel(this, p));
        }

        public GameVoxel GetVoxelAtGroundPos(Vector3 groundPos)
        {
            var index = Vector2.Modulate(groundPos.TakeXZ(), new Vector2(1 / voxelSize.X, 1 / voxelSize.Y));
            return voxels[index.Floor()];
        }

        public void ForEach(Action<GameVoxel, Point2> func)
        {
            voxels.ForEach(func);
        }

        public BoundingBox GetBoundingBox(Point2 gameVoxel)
        {
            var botLeft = Vector2.Modulate(gameVoxel, voxelSize).ToXZ(0);
            return new BoundingBox(botLeft, botLeft + voxelSize.ToXZ(0.1f));
        }
    }
}