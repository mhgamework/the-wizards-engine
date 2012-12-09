﻿using MHGameWork.TheWizards.CG.Math;

namespace MHGameWork.TheWizards.CG.Raytracing.Pipeline
{
    public class RayTrace
    {
        public int recurseDepth = 0;
        public float contribution = 1;
        public bool IsShadowRay = false;
        /// <summary>
        /// When this is true the trace will return the first hit found
        /// </summary>
        public bool FirstHit = false;

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
            if (!IsInRange(ref distance)) distance = null;
            
        }
        public bool IsInRange(ref float? distance)
        {
            if (distance == null) return false;
            if (distance < Start) return false;
            if (distance > End) return true;
            return true;
        }
    }
}