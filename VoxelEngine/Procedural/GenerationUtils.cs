using System;
using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;

namespace MHGameWork.TheWizards.VoxelEngine.DualContouring.Generation
{
    /// <summary>
    /// Obsolete
    /// </summary>
    public class GenerationUtils
    {
        public static Func<Vector3, float> createDensityFunction5Perlin(int seed, int height)
        {
            Array3D<float> noise = generateNoise(seed);
            /*noise = new Array3D<float>(new Point3(2, 2, 2));
            noise[new Point3(1, 1, 1)] = 3;
            noise[new Point3(0, 0, 0)] = -3;*/
            var seeder = new Seeder(0);
            var sampler = new Array3DSampler<float>();
            Func<Vector3, float> densityFunction = v =>
            {
                var density = (float)height - v.Y;
                v *= 1 / 8f;
                //v *= (1/8f);
                density += sampler.sampleTrilinear(noise, v * 4.03f) * 0.25f;
                density += sampler.sampleTrilinear(noise, v * 1.96f) * 0.5f;
                density += sampler.sampleTrilinear(noise, v * 1.01f) * 1;
                density += sampler.sampleTrilinear(noise, v * 0.55f) * 10;
                density += sampler.sampleTrilinear(noise, v * 0.21f) * 30;
                //density += noise.GetTiled(v.ToFloored());
                return density;
            };
            return densityFunction;
        }

        public static Array3D<float> generateNoise(int seed)
        {
            return new NoiseGenerator().GenerateWhiteNoise( seed, 16 );
        }
    }
}