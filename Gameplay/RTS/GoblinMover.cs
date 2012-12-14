using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.VoxelTerraining;
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

        public void Update()
        {
            position += TW.Graphics.Elapsed;
            if (position > path.Length)
                position = path.Length;
            var partOfPath = (int)position;
            var nextPart = partOfPath + 1;
            if (nextPart > path.Length)
            {
                goblin.Position = path[partOfPath];
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
            var star =new TerrainAStar();
            path = star.findPath(startVoxel, endVoxel).Select(o=>terrain.GetPositionOf(o)).ToArray();

        }



    }
}
