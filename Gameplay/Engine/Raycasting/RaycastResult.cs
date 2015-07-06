using System;
using SlimDX;

namespace MHGameWork.TheWizards.Engine.Raycasting
{
    /// <summary>
    /// Represents a result of a raycast. The object raycasted is stored, along with some optional data about the raycast
    /// </summary>
    public class RaycastResult
    {
        private float? distance;
        public float Distance
        {
            get
            {
                if (distance.HasValue == false) throw new Exception("There was no hit, so no distance available!");
                return distance.Value;
            }
        }

        public void Set(float? distance, object obj)
        {
            this.distance = distance;
            Object = obj;
        }

        /// <summary>
        /// The target of the raycast
        /// </summary>
        public object Object { get; private set; }

        public Vector3 V1 { get; set; }
        public Vector3 V2 { get; set; }
        public Vector3 V3 { get; set; }
        public Vector3 HitNormal { get; set; }



        /// <summary>
        /// Returns true if there was a hit, otherwise false.
        /// </summary>
        public bool IsHit
        {
            get { return distance.HasValue; }
        }

        public float? DistanceOrNull
        {
            get { return distance; }
        }

        public bool IsCloser(RaycastResult other)
        {
            if (IsHit)
            {
                if (!other.IsHit) return true;
                //TODO: is this really necessary?
                if (Math.Abs(Distance - other.Distance) < 0.001) return false;
                if (Distance < other.Distance) return true;
            }
            return false;
        }

        public int CompareTo(RaycastResult other)
        {
            if (IsHit)
            {
                if (!other.IsHit) return -1;
                if (Distance > other.Distance) return 1;
                if (Math.Abs(Distance - other.Distance) < 0.001) return 0;
                return -1;
            }

            if (other.IsHit == false) return 0;

            return 1;
        }

        public void CopyTo(RaycastResult target)
        {
            target.distance = distance;
            target.Object = Object;
            target.V1 = V1;
            target.V2 = V2;
            target.V3 = V3;
        }

    }
}
