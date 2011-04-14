using System;
using System.Collections.Generic;
using System.Text;

namespace TreeGenerator.help
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

    }
}
