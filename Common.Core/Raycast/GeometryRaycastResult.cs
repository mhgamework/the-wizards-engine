using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Raycast
{
    /// <summary>
    /// public class EntityRaycastResult<T> : Raycast.RaycastResult<T> where T : class
    /// Wierd construnction, T looks like it is always EditorEntity. But this is better because this way EntityRaycastResult 
    /// is independet of entity, and can become GeometryRaycastResult
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GeometryRaycastResult<T> : Raycast.RaycastResult<T> where T : class
    {
        public Vector3 Point;
        public Vector3 V1;
        public Vector3 V2;
        public Vector3 V3;

        public GeometryRaycastResult( float? dist, T nItem, Vector3 point, Vector3 v1, Vector3 v2, Vector3 v3 )
            : base( dist, nItem )
        {
            Point = point;
            V1 = v1;
            V2 = v2;
            V3 = v3;

        }

    }
}