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
        
        public byte NextByte( byte min, byte max )
        {
            return (byte)NextInt( min, max );
        }

        public Vector2 NextVector2(Vector2 min, Vector2 max)
        {
            Vector2 ret = new Vector2();
            ret.X = NextFloat( min.X, max.X );
            ret.Y = NextFloat( min.Y, max.Y );
            return ret;
        }

        public Vector3 NextVector3( Vector3 min, Vector3 max )
        {
            Vector3 ret = new Vector3();
            ret.X = NextFloat( min.X, max.X );
            ret.Y = NextFloat( min.Y, max.Y );
            ret.Z = NextFloat( min.Z, max.Z );
            return ret;
        }
        
        

        public Color NextColor()
        {
            Color color=new Color( NextByte( 0, 255 ), NextByte( 0, 255 ), NextByte( 0, 255 ) );
            return color;
        }



    }
}
