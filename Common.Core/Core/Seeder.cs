using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards
{
    public class Seeder
    {
        private Random random;
        private int seed;

        public int Seed
        {
            get { return seed; }
            //set { seed = value; }
        }

        public Seeder(int _seed)
        {
            seed = _seed;
            random = new Random(seed);
        }

        public int NextInt(int min, int max)
        {
            return random.Next(min, max);
        }

        public float NextFloat(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        public byte NextByte(byte min, byte max)
        {
            return (byte)NextInt(min, max);
        }

        public Vector2 NextVector2(Vector2 min, Vector2 max)
        {
            Vector2 ret = new Vector2();
            ret.X = NextFloat(min.X, max.X);
            ret.Y = NextFloat(min.Y, max.Y);
            return ret;
        }

        public Vector3 NextVector3(Vector3 min, Vector3 max)
        {
            if (min.X > max.X)
                throw new InvalidOperationException();
            if (min.Y > max.Y)
                throw new InvalidOperationException();
            if (min.Z > max.Z)
                throw new InvalidOperationException();

            Vector3 ret = new Vector3();
            ret.X = NextFloat(min.X, max.X);
            ret.Y = NextFloat(min.Y, max.Y);
            ret.Z = NextFloat(min.Z, max.Z);
            return ret;
        }



        public Color NextColor()
        {
            Color color = new Color(NextByte(0, 255), NextByte(0, 255), NextByte(0, 255));
            return color;
        }

        public Color NextColor(byte R1, byte R2, byte G1, byte G2, Byte B1, byte B2)
        {
            byte R, G, B;
            if (R1 > R2)
                R = NextByte(R2, R1);
            else
                R = NextByte(R1, R2);
            if (G1 > G2)
                G = NextByte(G2, G1);
            else
                G = NextByte(G1, G2);
            if (B1 > B2)
                B = NextByte(B2, B1);
            else
                B = NextByte(B1, B2);

            Color color = new Color(R, G, B);
            return color;
        }
        public Color NextColor(Vector2 R, Vector2 G, Vector2 B)
        {
            Color color = new Color(NextByte((byte)R.X, (byte)R.Y), NextByte((byte)G.X, (byte)G.Y), NextByte((byte)B.X, (byte)B.Y));
            return color;
        }
        public Color NextColor(Vector2 R, Vector2 G, Vector2 B, Vector2 A)
        {
            Color color = new Color(NextByte((byte)R.X, (byte)R.Y), NextByte((byte)G.X, (byte)G.Y), NextByte((byte)B.X, (byte)B.Y), NextByte((byte)A.X, (byte)A.Y));
            return color;
        }

        /// <summary>
        /// When called ensures that on average each ('averageInterval' seconds) the function returns true,
        /// as long as the elapsed value is small enough compared to the average interval 
        /// (otherwise multiple events could occur in a single interval)
        /// Uses Poisson distribution for 1 or more events inside a single interval
        /// TODO: only executes action once, could execute multiple times to be more correct 
        /// </summary>
        /// <returns></returns>
        public void EachRandomInterval(float averageInterval, Action action, float elapsed)
        {
            var count = PoissonSmall(elapsed / averageInterval);
            for (int i = 0; i < count; i++)
            {
                action();
            }
        }

        /// <summary>
        /// TODO: returns a maximum of 100, otherwise this function might get to slow and
        /// a better implementation is required
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns></returns>
        public int PoissonSmall(double lambda)
        {
            // Algorithm due to Donald Knuth, 1969.
            double p = 1.0, L = Math.Exp(-lambda);
            int k = 0;
            do
            {
                k++;
                p *= NextFloat(0, 1); //GetUniform();
            }
            while (p > L && k < 99);
            return k - 1;
        }

    }
}
