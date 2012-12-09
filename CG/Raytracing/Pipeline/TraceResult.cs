using MHGameWork.TheWizards.CG.Math;

namespace MHGameWork.TheWizards.CG.Raytracing.Pipeline
{
    public struct TraceResult
    {
        public float? Distance;
        public ShadeDelegate ShadeDelegate;
        public GeometryInput GeometryInput;

        public bool IsHit
        {
            get { return Distance != null; }
        }

        /// <summary>
        /// Sets this result to the closest of given new result and the current closest result
        /// </summary>
        /// <param name="newResult"></param>
        /// <returns></returns>
        public bool AddResult(ref TraceResult newResult)
        {
            bool ret =  isCloser(ref Distance,  ref newResult.Distance);
            if (ret)
            {
                copyFrom(ref newResult);
            }
            return ret;
        }

        private static bool isCloser(ref float? closestHit,  ref float? newHit)
        {
            bool closer = false;

            if (!newHit.HasValue)
                closer = false;
            else if (!closestHit.HasValue)
                closer = true;
            else if (closestHit.Value > newHit.Value)
                closer = true;

            return closer;
        }

        private void copyFrom(ref TraceResult result)
        {
            Distance = result.Distance;
            ShadeDelegate = result.ShadeDelegate;
            GeometryInput = result.GeometryInput;
        }

        public Color4 Shade(ref RayTrace trace)
        {
            return ShadeDelegate(ref GeometryInput,ref trace);
        }
    
    }
}