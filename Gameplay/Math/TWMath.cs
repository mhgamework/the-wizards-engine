using System;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards
{
    public class TWMath
    {
        /// <summary>
        /// Looparound modulo, works with negative numbers
        /// </summary>
        public static int nfmod(int a, int b)
        {
            return (a - b * (int)Math.Floor(a / (double)b));
        }
        /// <summary>
        /// Looparound modulo, works with negative numbers
        /// </summary>
        public static float nfmod(float a, float b)
        {
            return (float)(a - b * Math.Floor(a / b));
        }



        /// <summary>
        /// Performs trilinear interpolation on the q-values (qXYZ) which are positioned at the corners of a cube from (0,0,0) to (1,1,1)
        /// The factor is the sample point inside this unit cube.
        /// </summary>
        public static float triLerp(Vector3 factor, float q000, float q100, float q001, float q101, float q010, float q110,
                                     float q011, float q111)
        {
            var x00 = MathHelper.Lerp(q000, q100, factor.X);
            var x01 = MathHelper.Lerp(q001, q101, factor.X);
            var x10 = MathHelper.Lerp(q010, q110, factor.X);
            var x11 = MathHelper.Lerp(q011, q111, factor.X);

            var y0 = MathHelper.Lerp(x00, x10, factor.Y);
            var y1 = MathHelper.Lerp(x01, x11, factor.Y);


            var z = MathHelper.Lerp(y0, y1, factor.Z);
            return z;
        }


    }
}