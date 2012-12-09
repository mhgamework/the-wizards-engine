using System;

namespace MHGameWork.TheWizards.CG.Raytracing
{
    public struct TraceResult
    {
        public float? Distance;
        public ShadeDelegate ShadeDelegate;

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
            return changeWhenCloser(ref Distance, ref ShadeDelegate, ref newResult.Distance, ref newResult.ShadeDelegate);
        }

        public static bool changeWhenCloser(ref float? closestHit, ref ShadeDelegate closestCommand, ref float? newHit, ref ShadeDelegate newCommand)
        {
            bool closer = false;

            if (!newHit.HasValue)
                closer = false;
            else if (!closestHit.HasValue)
                closer = true;
            else if (closestHit.Value > newHit.Value)
                closer = true;

            if (closer)
            {
                closestHit = newHit;
                closestCommand = newCommand;


            }
            return closer;
        }
    
    }
}