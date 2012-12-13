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
        }

        private Vector3[] path;
        private float position;
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
            //var startVoxel = TW.Data.GetSingleton<VoxelTerrain>().GetVoxelAt(goblin.Position);
            //var endVoxel = TW.Data.GetSingleton<VoxelTerrain>().GetVoxelAt(pos);
            path = new[] {goblin.Position, pos};
            //var path = findPath(startVoxel, endVoxel);

        }



    }
}
