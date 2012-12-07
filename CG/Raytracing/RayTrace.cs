using MHGameWork.TheWizards.CG.Math;

namespace MHGameWork.TheWizards.CG.Raytracing
{
    public class RayTrace
    {
        public int recurseDepth = 0;
        public float contribution = 1;
        public bool IsShadowRay = false;

        public Ray Ray;
        public float Start;
        public float End;

        public RayTrace(Ray ray, float start, float end)
        {
            this.Ray = ray;
            this.Start = start;
            this.End = end;
        }

       
        /// <summary>
        /// Utility function that sets the distance to null when it is not in range of this raytrace
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public void SetNullWhenNotInRange( ref float? distance)
        {
            if (distance == null) return;

            if (distance < Start) distance = null;
            if (distance > End) distance = null;
        }
    }
}