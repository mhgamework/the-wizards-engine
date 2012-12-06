using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Math;

namespace MHGameWork.TheWizards.CG.Sampling
{
    public class JitteredSampler
    {
        private int size;

        private float cellSize;

        private Random r = new Random(654);

        public JitteredSampler(int size)
        {
            this.size = size;
            cellSize = 1f / size;
        }

        public int SampleCount
        {
            get { return size * size; }
        }

        public IEnumerable<Vector2> GenerateSamples()
        {
            for (float x = 0; x < 0.9999; x+=cellSize)
                for (float y = 0; y < 0.999; y+=cellSize)
                {
                    yield return new Vector2(x + (float)r.NextDouble() * cellSize, y + (float)r.NextDouble() * cellSize);
                }
        }
    }
}
