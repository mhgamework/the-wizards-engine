// Type: Microsoft.Xna.Framework.MathHelper
// Assembly: Microsoft.Xna.Framework, Version=2.0.0.0, Culture=neutral, PublicKeyToken=6d5c3888ef60e27d
// Assembly location: C:\Windows\assembly\GAC_32\Microsoft.Xna.Framework\2.0.0.0__6d5c3888ef60e27d\Microsoft.Xna.Framework.dll

using System;
using SlimDX;

namespace Microsoft.Xna.Framework
{
    public static class MathHelper
    {
        // Fields
        public const float E = 2.718282f;
        public const float Log10E = 0.4342945f;
        public const float Log2E = 1.442695f;
        public const float Pi = 3.141593f;
        public const float PiOver2 = 1.570796f;
        public const float PiOver4 = 0.7853982f;
        public const float TwoPi = 6.283185f;

        // Methods
        public static float Barycentric(float value1, float value2, float value3, float amount1, float amount2)
        {
            return ((value1 + (amount1 * (value2 - value1))) + (amount2 * (value3 - value1)));
        }

        public static float CatmullRom(float value1, float value2, float value3, float value4, float amount)
        {
            float num = amount * amount;
            float num2 = amount * num;
            return (0.5f * ((((2f * value2) + ((-value1 + value3) * amount)) + (((((2f * value1) - (5f * value2)) + (4f * value3)) - value4) * num)) + ((((-value1 + (3f * value2)) - (3f * value3)) + value4) * num2)));
        }

        public static float Clamp(float value, float min, float max)
        {
            value = (value > max) ? max : value;
            value = (value < min) ? min : value;
            return value;
        }

        public static float Distance(float value1, float value2)
        {
            return Math.Abs((float)(value1 - value2));
        }

        public static float Hermite(float value1, float tangent1, float value2, float tangent2, float amount)
        {
            float num3 = amount;
            float num = num3 * num3;
            float num2 = num3 * num;
            float num7 = ((2f * num2) - (3f * num)) + 1f;
            float num6 = (-2f * num2) + (3f * num);
            float num5 = (num2 - (2f * num)) + num3;
            float num4 = num2 - num;
            return ((((value1 * num7) + (value2 * num6)) + (tangent1 * num5)) + (tangent2 * num4));
        }

        public static float Lerp(float value1, float value2, float amount)
        {
            return (value1 + ((value2 - value1) * amount));
        }

        public static float Max(float value1, float value2)
        {
            return Math.Max(value1, value2);
        }

        public static float Min(float value1, float value2)
        {
            return Math.Min(value1, value2);
        }

        public static float SmoothStep(float value1, float value2, float amount)
        {
            float num = Clamp(amount, 0f, 1f);
            return Lerp(value1, value2, (num * num) * (3f - (2f * num)));
        }

        public static float ToDegrees(float radians)
        {
            return (radians * 57.29578f);
        }

        public static float ToRadians(float degrees)
        {
            return (degrees * 0.01745329f);
        }




        private static Vector3 up;
        private static Vector3 down;
        private static Vector3 left;
        private static Vector3 right;
        private static Vector3 forward;
        private static Vector3 backward;
        private static Vector3 one;


        public static Vector3 Up
        {
            get { return up; }
        }

        public static Vector3 Down
        {
            get { return down; }
        }

        public static Vector3 Left
        {
            get { return left; }
        }

        public static Vector3 Right
        {
            get { return right; }
        }

        public static Vector3 Forward
        {
            get { return forward; }
        }

        public static Vector3 Backward
        {
            get { return backward; }
        }

        public static Vector3 One
        {
            get { return one; }
        }


        static MathHelper()
        {
            up = new Vector3(0, 1, 0);
            down = new Vector3(0, -1, 0);
            left = new Vector3(-1, 0, 0);
            right = new Vector3(1, 0, 0);
            forward = new Vector3(0, 0, -1);
            backward = new Vector3(0, 0, 1);
            one = new Vector3(1, 1, 1);
        }



        //MHGW

        /// <summary>
        /// Performs a modulo operation that returns a value in 0 .. m-1
        /// </summary>
        /// <param name="x"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static float Modulo(float x, float m)
        {
            return (x % m + m) % m;
        }
        public static int Modulo(int x, int m)
        {
            return (x % m + m) % m;
        }
    }
}
