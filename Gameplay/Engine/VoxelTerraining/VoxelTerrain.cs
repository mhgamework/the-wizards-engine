using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.CG.Spatial;
using MHGameWork.TheWizards.Data;
using SlimDX;

namespace MHGameWork.TheWizards.Engine.VoxelTerraining
{
    /// <summary>
    /// Responsible for storing data for the voxel terrain for the entire world
    /// Uses chunks to store and access data
    /// </summary>
    [ModelObjectChanged]
    public class VoxelTerrain : EngineModelObject
    {
        /// <summary>
        /// This creates a new wrapper VoxelBlock object!!!
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public VoxelBlock GetVoxelAt(Vector3 pos)
        {
            foreach (VoxelTerrainChunk chunk in TW.Data.Objects.Where(o => o is VoxelTerrainChunk))
            {
                var relative = pos - chunk.WorldPosition;
                relative *= 1 / (chunk.NodeSize);
                for (int i = 0; i < 3; i++)
                    relative[i] = (int)relative[i];
                    
                var point = new Point3(relative);
                if (chunk.InGrid(point))
                {
                    return getChunkVoxelWrapper(chunk, point);
                }
            }

            return null;
        }

        private static VoxelBlock getChunkVoxelWrapper(VoxelTerrainChunk chunk, Point3 point)
        {
            var voxelInternal = chunk.GetVoxelInternal(ref point);
            if (voxelInternal == null)
                return null;
            return new VoxelBlock(point, chunk, voxelInternal);
        }

        public Vector3 GetPositionOf(VoxelBlock block)
        {
            return block.Position.ToVector3() * block.TerrainChunk.NodeSize + block.TerrainChunk.WorldPosition;

        }


        public VoxelBlock Raycast(Ray ray)
        {
            VoxelBlock dmqklj;
            return Raycast(ray, out dmqklj);
        }

        public VoxelBlock Raycast(Ray ray, out VoxelBlock emptyTargetedBlock)
        {
            VoxelBlock last = null;
            VoxelBlock ret = null;
            var traverser = new GridTraverser();


            float? closest = null;


            foreach (VoxelTerrainChunk terr in TW.Data.Objects.Where(o => o is VoxelTerrainChunk))
            {
                var trace = new RayTrace();
                trace.Ray = ray;

                float? dist = trace.Ray.xna().Intersects(terr.GetBoundingBox().xna());
                if (!dist.HasValue) continue;
                if (closest.HasValue && closest.Value < dist.Value)
                    continue;

                trace.Start = dist.Value + 0.001f;



                traverser.NodeSize = terr.NodeSize;
                traverser.GridOffset = terr.WorldPosition;

                //TODO: fix multiple terrains 


                var hit = false;


                VoxelTerrainChunk terr1 = terr;
                traverser.Traverse(trace, delegate(Point3 arg)
                {
                    if (!terr1.InGrid(arg)) return true;

                    var voxelBlock = terr1.GetVoxelInternal(ref arg);
                    if (voxelBlock == null) return false;
                    if (voxelBlock.Filled)
                    {
                        hit = true;
                        ret = getChunkVoxelWrapper(terr1,arg);
                        return true;
                    }
                    last = getChunkVoxelWrapper(terr1, arg);
                    return false;
                });


                if (hit)
                {
                    closest = dist;
                }


            }
            emptyTargetedBlock = last;
            return ret;
        }
    }
}
