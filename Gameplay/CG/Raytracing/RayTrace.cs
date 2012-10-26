using SlimDX;

namespace MHGameWork.TheWizards.CG
{
    public class RayTrace
    {
        private Ray ray;
        private float start;
        private float end;

        public RayTrace(Ray ray, float start, float end)
        {
            this.ray = ray;
            this.start = start;
            this.end = end;
        }

        public Ray Ray
        {
            get { return ray; }
        }

        public float Start
        {
            get { return start; }
        }

        public float End
        {
            get { return end; }
        }
    }
}