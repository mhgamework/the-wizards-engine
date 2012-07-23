using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Raycast
{
    public class SimpleRaycaster<T>
    {

        public T ClosestObject { get; set; }

        public float? ClosestDistance { get; set; }

        public void AddResult(float? distance, T obj)
        {
            if (!distance.HasValue) return;
            if (!ClosestDistance.HasValue || distance.Value < ClosestDistance.Value)
            {
                ClosestDistance = distance;
                ClosestObject = obj;
            }
        }

        public void Reset()
        {
            ClosestObject = default(T);
            ClosestDistance = null;
        }



    }
}
