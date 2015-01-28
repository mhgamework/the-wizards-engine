﻿using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    /// <summary>
    /// Intersectable object
    /// </summary>
    public interface IIntersectableObject
    {
        /// <summary>
        /// Returns true when given point is inside the object
        /// </summary>
        bool IsInside(Vector3 v);

        /// <summary>
        /// Finds the intersectin with the surface of the object, between start and end.
        /// The returnvalue is the normal of the intersection point (XYZ), pointing away from the object,
        /// and lerp value between 0 and 1 from start to end (W)
        /// 
        /// If there are zero or more than 1 intersection points between start and end, the methods behaviour is undefined.
        /// </summary>
        Vector4 GetIntersection(Vector3 start, Vector3 end);

    }
}