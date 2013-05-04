using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
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
        private float next = 0;
        public void Update()
        {
            if (next < TW.Graphics.TotalRunTime + 1)
            {
                updatePath();
                next = TW.Graphics.TotalRunTime + 1;
            }

            if (path != null)
            {
                //goblin.Position = path[0];

                var diff = path[1] - goblin.Position;
                var delta = TW.Graphics.Elapsed * Vector3.Normalize(diff);
                if (diff.Length() < delta.Length()) delta = diff;
                goblin.Position += delta;
            }
            return;
            position += TW.Graphics.Elapsed;

            if (position > path.Length - 1)
                position = path.Length - 1;

            var partOfPath = (int)position;
            var nextPart = partOfPath + 1;
            if (nextPart >= path.Length)
            {
                goblin.Position = path[partOfPath] + MathHelper.One * 0.5f;
                finished = true;
                return;
            }
            goblin.Position = Vector3.Lerp(path[partOfPath], path[nextPart], position - (int)position) + MathHelper.One * 0.5f;
        }

        private void updatePath()
        {
            path = null;

            var provider = new GridConnectionProvider();
            provider.Grid = TW.Data.GetSingleton<NavigableGrid2DData>().Grid;
            provider.Size = 2;
            if (provider.Grid == null) return;

            var s = provider.GetVertex((goblin.Position.TakeXZ() - new Vector2(0.0f)) / provider.Grid.NodeSize);
            var e = provider.GetVertex((goblin.Goal.TakeXZ() - new Vector2(0.0f)) / provider.Grid.NodeSize);
            if (s == null || e == null) return;
            if (s.Position == e.Position)
            {
                path = new[] { goblin.Goal, goblin.Goal };
                path[0].Y = 0;
                path[1].Y = 0;
                return;
            }
            TW.Graphics.LineManager3D.AddCenteredBox(((e.Position + new Vector2(0.5f, 0.5f)) / 2).ToXZ(), 0.3f, new Color4(1, 0, 0));

            var finder = new PathFinder2D<Vertex2D>();
            finder.ConnectionProvider = provider;
            finder.StopCondition = v => finder.GetCurrentCost(v) > 50;
            var vPath = finder.FindPath(s, e);
            if (vPath == null) return;
            path = vPath.Select(v => (v.Position.ToXZ() + new Vector3(0.5f, 0, 0.5f)) * provider.Grid.NodeSize).ToArray();



            //// find goblin pos in terrain
            //var startVoxel = terrain.GetVoxelAt(goblin.Position);
            //var endVoxel = terrain.GetVoxelAt(goblin.Goal);
            ////path = new[] {goblin.Position, pos};
            //path = null;
            //if (startVoxel == null || endVoxel == null) return;
            //var star = new TerrainAStar();
            //path = star.findPath(endVoxel, startVoxel).Select(o => terrain.GetPositionOf(o)).ToArray();
            //finished = false;
            //position = 0;
        }

        public void MoveTo(Vector3 pos)
        {
            goblin.Goal = pos;

        }



    }
}
