using System;
using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;

namespace MHGameWork.TheWizards.VoxelEngine.DualContouring.Generation
{
    /// <summary>
    /// Contains noise functions for procedural generation
    /// </summary>
    public class NoiseGenerator
    {
        private static Array3DSampler<float> sampler;

        private Array3D<float> perlinNoise;

        public NoiseGenerator()
        {
            sampler = new Array3DSampler<float>();

            perlinNoise = GenerateWhiteNoise(0);
        }

        public Array3D<float> GenerateWhiteNoise(int seed, int size = 16)
        {
            var ret = new Array3D<float>(new Point3(size, size, size));

            var r = new Seeder(seed);

            ret.ForEach((val, p) =>
            {
                ret[p] = r.NextFloat(-1, 1);
            });

            return ret;
        }


        public float CalculatePerlinNoise1D(float x)
        {
            var floor = (int)x;
            var f = x - floor;
            var a = TWMath.nfmod(floor + 0, perlinNoise.Size.X);
            var b = TWMath.nfmod(floor + 1, perlinNoise.Size.X);
            return MathHelper.Lerp(perlinNoise.GetFast(a, 0, 0), perlinNoise.GetFast(b, 0, 0), f);
        }
        public float CalculatePerlinNoise(Vector3 pos)
        {
            // Non-linear version:
            var min = pos.ToFloored();
            var frac = pos - min;

            // Quintic smoothstep interpolation function: 6t^5 - 15t^4 + 10t^3
            for (int i = 0; i < 3; i++)
            {
                var uvFrac = frac[i];
                frac[i] = (((6 * uvFrac) - 15) * uvFrac + 10) * uvFrac * uvFrac * uvFrac;
            }

            /*var q000 = noise.GetTiled(new Point3(0, 0, 0) + min);
            var q100 = noise.GetTiled(new Point3(1, 0, 0) + min);
            var q010 = noise.GetTiled(new Point3(0, 1, 0) + min);
            var q001 = noise.GetTiled(new Point3(0, 0, 1) + min);
            var q110 = noise.GetTiled(new Point3(1, 1, 0) + min);
            var q011 = noise.GetTiled(new Point3(0, 1, 1) + min);
            var q101 = noise.GetTiled(new Point3(1, 0, 1) + min);
            var q111 = noise.GetTiled(new Point3(1, 1, 1) + min);*/
            var x0 = TWMath.nfmod(min.X + 0, perlinNoise.Size.X); ;
            var y0 = TWMath.nfmod(min.Y + 0, perlinNoise.Size.Y);
            var z0 = TWMath.nfmod(min.Z + 0, perlinNoise.Size.Z);

            var x1 = TWMath.nfmod(x0 + 1, perlinNoise.Size.X);
            var y1 = TWMath.nfmod(y0 + 1, perlinNoise.Size.Y);
            var z1 = TWMath.nfmod(z0 + 1, perlinNoise.Size.Z);
            var q000 = perlinNoise.GetFast(x0, y0, z0);
            var q100 = perlinNoise.GetFast(x1, y0, z0);
            var q010 = perlinNoise.GetFast(x0, y1, z0);
            var q001 = perlinNoise.GetFast(x0, y0, z1);
            var q110 = perlinNoise.GetFast(x1, y1, z0);
            var q011 = perlinNoise.GetFast(x0, y1, z1);
            var q101 = perlinNoise.GetFast(x1, y0, z1);
            var q111 = perlinNoise.GetFast(x1, y1, z1);

            var ret = TWMath.triLerp(frac.dx(), q000, q100, q001, q101, q010, q110, q011, q111);
            return ret;

            //return sampler.sampleTrilinear(perlinNoise, p);
        }
    }
}