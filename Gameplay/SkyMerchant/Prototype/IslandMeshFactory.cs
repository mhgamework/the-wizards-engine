using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.SkyMerchant.Voxels;
using MHGameWork.TheWizards.SkyMerchant._Tests.Development;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.Parts
{
    public class IslandMeshFactory
    {
        private Dictionary<int, IMesh> cache = new Dictionary<int, IMesh>();

        private readonly VoxelMeshBuilder c;

        public IslandMeshFactory(VoxelMeshBuilder c)
        {
            this.c = c;
        }

        public IMesh GetMesh(int seed)
        {
            if (!cache.ContainsKey(seed))
            {
                cache[seed] = CreateMesh(seed);
            }
            return cache[seed];
        }

        public IMesh CreateMesh(int seed)
        {
            var r = new Random(seed);
            var gen = new IslandGenerater(r);

            var island = gen.GenerateIsland(r.Next(7, 20));
            var mesh = c.BuildMesh(island);

            return mesh;
        }
    }
}