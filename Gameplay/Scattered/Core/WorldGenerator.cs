using System;
using System.Collections.Generic;
using System.Threading;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class WorldGenerator
    {
        private readonly Level level;
        private readonly Random random;

        public WorldGenerator(Level level, Random random)
        {
            this.level = level;
            this.random = random;
            samples = generateSamples();
        }

        private int[] generateSamples()
        {
            var ret = new int[stratifiedSize * stratifiedSize];
            for (int i = 0; i < ret.Length * 10; i++)
            {
                var a = random.Next(0, ret.Length - 1);
                var b = random.Next(0, ret.Length - 1);
                var swap = ret[a];
                ret[a] = ret[b];
                ret[b] = swap;
            }
            return ret;
        }

        public void Generate()
        {
            for (int i = 0; i < 100; i++)
            {
                var pos = (incorrectStratifiedSample() *3).ToXZ();
                level.CreateNewIsland(pos);
            }
        }


        private int[] samples;

        private int samplePos = 0;
        private int stratifiedSize = 16;

        /// <summary>
        /// Returns between 0 and stratifiedsize;
        /// </summary>
        private Vector2 incorrectStratifiedSample()
        {
            float x = samplePos / stratifiedSize;
            float y = samplePos % stratifiedSize;

            x += (float)random.NextDouble();
            y += (float)random.NextDouble();
            return new Vector2(x, y);
        }
    }


}