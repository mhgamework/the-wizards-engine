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

            /*var q000 = noise.GetTiled(new Point3(0, 0, 0) + min);
            var q100 = noise.GetTiled(new Point3(1, 0, 0) + min);
            var q010 = noise.GetTiled(new Point3(0, 1, 0) + min);
            var q001 = noise.GetTiled(new Point3(0, 0, 1) + min);
            var q110 = noise.GetTiled(new Point3(1, 1, 0) + min);
            var q011 = noise.GetTiled(new Point3(0, 1, 1) + min);
            var q101 = noise.GetTiled(new Point3(1, 0, 1) + min);
            var q111 = noise.GetTiled(new Point3(1, 1, 1) + min);*/
            var x0 = TWMath.nfmod(min.X + 0, noise.Size.X); ;
            var y0 = TWMath.nfmod(min.Y + 0, noise.Size.Y);
            var z0 = TWMath.nfmod(min.Z + 0, noise.Size.Z);

            var x1 = TWMath.nfmod(x0 + 1, noise.Size.X);
            var y1 = TWMath.nfmod(y0 + 1, noise.Size.Y);
            var z1 = TWMath.nfmod(z0 + 1, noise.Size.Z);
            var q000 = noise.GetFast(x0, y0, z0);
            var q100 = noise.GetFast(x1, y0, z0);
            var q010 = noise.GetFast(x0, y1, z0);
            var q001 = noise.GetFast(x0, y0, z1);
            var q110 = noise.GetFast(x1, y1, z0);
            var q011 = noise.GetFast(x0, y1, z1);
            var q101 = noise.GetFast(x1, y0, z1);
            var q111 = noise.GetFast(x1, y1, z1);

            var ret = TWMath.triLerp(f, q000, q100, q001, q101, q010, q110, q011, q111);
            return ret;
        }
    }
}