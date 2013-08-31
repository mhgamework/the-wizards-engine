using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards
{
    [Obsolete ("New version is called DXSeeder")]
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
        public SlimDX.Vector2 NextVector2(SlimDX.Vector2 min, SlimDX.Vector2 max)
        {
            SlimDX.Vector2 ret = new SlimDX.Vector2();
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

    }
}
