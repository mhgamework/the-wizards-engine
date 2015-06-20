using System;

namespace MHGameWork.TheWizards
{
    public class TWMath
    {
        /// <summary>
        /// Looparound modulo, works with negative numbers
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int nfmod(int a, int b)
        {
            return (a - b * (int)Math.Floor(a / (double)b));
        }
        public static float nfmod(float a, float b)
        {
            return (float)(a - b * Math.Floor(a / b));
        }
    }
}