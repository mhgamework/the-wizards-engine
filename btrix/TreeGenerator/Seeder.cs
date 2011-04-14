using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TreeGenerator.help
{
    [Obsolete("This class is an old version. The new version is in the Common project")]
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
        public int NextInt(Range range)
        {
            
            return (int)NextFloat(range.Min, range.Max + 1);
        }
        public float NextFloat(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }
        public float NextFloat(Range range)
        {
            return range.Min + (float)random.NextDouble() * (range.Max - range.Min);
        }
        
        public byte NextByte( byte min, byte max )
        {
            return (byte)NextInt( min, max );
        }
        public Color NextColor()
        {
            Color color=new Color( NextByte( 0, 255 ), NextByte( 0, 255 ), NextByte( 0, 255 ) );
            return color;
        }
        public Color NextColor(byte R1,byte R2, byte G1,byte G2, Byte B1,byte B2)
        {
            byte R, G, B;
            if (R1 > R2)
               R= NextByte(R2, R1);
            else
                R = NextByte(R1, R2);
            if (G1 > G2)
               G= NextByte(G2, G1);
            else
               G= NextByte(G1, G2);
            if (B1 >B2)
                B=NextByte(B2,B1);
            else
               B= NextByte(B1, B2);

            Color color = new Color(R, G, B);
            return color;
        }
        public Color NextColor(Vector2 R, Vector2 G, Vector2 B)
        {
            Color color = new Color(NextByte((byte)R.X, (byte)R.Y), NextByte((byte)G.X, (byte)G.Y), NextByte((byte)B.X, (byte)B.Y));
            return color;
        }
        public Color NextColor(Vector2 R, Vector2 G, Vector2 B,Vector2 A)
        {
            Color color = new Color(NextByte((byte)R.X, (byte)R.Y), NextByte((byte)G.X, (byte)G.Y), NextByte((byte)B.X, (byte)B.Y), NextByte((byte)A.X, (byte)A.Y));
            return color;
        }
        

        /// <summary>
        /// Note: To be verified
        /// TODO: make this flow through, so that when spreading is > 1, code still works.
        /// (if the deviation is ex 5 and we are on the last index, we want the resulting float to be at the beginning)
        /// </summary>
        /// <param name="spreading"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public float NextFloat(RangeSpreading spreading, int index, int count)
        {
            if (count==1)
            {
                return NextFloat(spreading.Min, spreading.Max);
            }
            float oneOverCount = 1f / count;
            float deviation = NextFloat(-oneOverCount * spreading.Deviation, oneOverCount * spreading.Deviation);
            return (index * oneOverCount + deviation) * (spreading.Max - spreading.Min) + spreading.Min;

        }

        public Vector3 NextVector3(Vector3 pos1, Vector3 pos2)
        {
            Vector3 temp=new Vector3();
            if (pos1.X < pos2.X)
            { temp.X = NextFloat(pos1.X, pos2.X); }
            else { temp.X = NextFloat(pos2.X, pos1.X); }
            if (pos1.Y < pos2.Y)
            { temp.Y = NextFloat(pos1.Y, pos2.Y); }
            else { temp.Y = NextFloat(pos2.Y, pos1.Y); }
            if (pos1.Z < pos2.Z)
            { temp.Z = NextFloat(pos1.Z, pos2.Z); }
            else { temp.Z = NextFloat(pos2.Z, pos1.Z); }

            return temp;
            
        }

    }
}
