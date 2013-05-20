using System;
using MHGameWork.TheWizards.Engine.Raycasting;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Selecting
{
    /// <summary>
    /// Something that is selectable, internal use
    /// </summary>
    public class Selectable
    {
        private readonly Func<Ray,RaycastResult> intersect;

        public Selectable(Func<Ray, RaycastResult> intersect, Object obj)
        {
            this.intersect = intersect;
            this.Object = obj;
        }

        public object Object { get; set; }

        /// <summary>
        /// TODO: maybe remove raycastresult?
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public RaycastResult Intersects(Ray ray)
        {
            return intersect(ray);
        }
    }
}