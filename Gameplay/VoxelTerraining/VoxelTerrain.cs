﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.CG.Spatial;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.VoxelTerraining
{
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
            var voxelInternal = chunk.GetVoxelInternal(point);
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

                    var voxelBlock = terr1.GetVoxelInternal(arg);
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
