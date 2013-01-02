using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using SlimDX;

namespace MHGameWork.TheWizards.RTS
{
    class GoblinMover
    {
        private readonly Goblin goblin;

        public GoblinMover(Goblin goblin)
        {
            this.goblin = goblin;
            terrain = TW.Data.GetSingleton<VoxelTerrain>();
        }

        private Vector3[] path;
        private float position;
        private VoxelTerrain terrain;
        private bool finished = true;
        public void Update()
        {
            if (finished)
                return;
            if (path == null) return;
            position += TW.Graphics.Elapsed;
            if (position > path.Length - 1)
                position = path.Length - 1;
            var partOfPath = (int)position;
            var nextPart = partOfPath + 1;
            if (nextPart >= path.Length)
            {
                goblin.Position = path[partOfPath];
                finished = true;
                return;
            }
            goblin.Position = Vector3.Lerp(path[partOfPath], path[nextPart], position - (int)position);
        }

        public void MoveTo(Vector3 pos)
        {
            // find goblin pos in terrain
            var startVoxel = terrain.GetVoxelAt(goblin.Position);
            var endVoxel = terrain.GetVoxelAt(pos);
            //path = new[] {goblin.Position, pos};
            path = null;
            if (startVoxel == null || endVoxel == null) return;
            if (endVoxel.Filled) return;
            var star = new TerrainAStar();
            var tempPath = star.findPath(startVoxel,endVoxel);
            if (tempPath != null)
            {
                path = tempPath.Reverse<VoxelBlock>().Select(o => terrain.GetPositionOf(o) + MathHelper.One * 0.5f).ToArray();
                path[0] = goblin.Position;

                foreach (var voxelBlock in tempPath)
                {
                    if (voxelBlock.Filled ) throw new InvalidOperationException();
                }

            }



            finished = false;
            position = 0;
        }



    }
}
