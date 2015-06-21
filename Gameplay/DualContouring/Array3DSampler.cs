using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    /// <summary>
    /// Sampler (like texture sampler) for the Array3D class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Array3DSampler<T>
    {

        public float sampleTrilinear(Array3D<float> noise, Vector3 pos)
        {

            var min = pos.ToFloored();
            var f = pos - min;

            var q000 = noise.GetTiled(new Point3(0, 0, 0) + min);
            var q100 = noise.GetTiled(new Point3(1, 0, 0) + min);
            var q010 = noise.GetTiled(new Point3(0, 1, 0) + min);
            var q001 = noise.GetTiled(new Point3(0, 0, 1) + min);
            var q110 = noise.GetTiled(new Point3(1, 1, 0) + min);
            var q011 = noise.GetTiled(new Point3(0, 1, 1) + min);
            var q101 = noise.GetTiled(new Point3(1, 0, 1) + min);
            var q111 = noise.GetTiled(new Point3(1, 1, 1) + min);


            var z = TWMath.triLerp(f, q000, q100, q001, q101, q010, q110, q011, q111);
            return z;
        }
    }
}