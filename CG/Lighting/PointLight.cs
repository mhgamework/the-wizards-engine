using System;
using MHGameWork.TheWizards.CG.Math;

namespace MHGameWork.TheWizards.CG.Lighting
{
    public class PointLight
    {
        public Vector3 Position;
        public float Radius;
        public float Intensity;

        public int NumSamples
        {
            get { return 1; }
        }

        private Random r = new Random(123);
        public Vector3 SamplePosition()
        {
            return Position + new Vector3(1, 0, 0)*(float)r.NextDouble()*0;
        }
    }
}
