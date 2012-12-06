using MHGameWork.TheWizards.CG.Math;

namespace MHGameWork.TheWizards.CG.Raytracing
{
    public class RayTrace
    {
        public Ray Ray;
        private float start;
        private float end;

        public RayTrace(Ray ray, float start, float end)
        {
            this.Ray = ray;
            this.start = start;
            this.end = end;
        }

       
        public float Start
        {
            get { return start; }
        }

        public float End
        {
            get { return end; }
        }

        /// <summary>
        /// Utility function that sets the distance to null when it is not in range of this raytrace
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public void SetNullWhenNotInRange( ref float? distance)
        {
            if (distance == null) return;

            if (distance < start) distance = null;
            if (distance > end) distance = null;
        }
    }
}