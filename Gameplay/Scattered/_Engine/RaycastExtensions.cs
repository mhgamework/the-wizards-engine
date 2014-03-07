using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Engine.Raycasting;
using MHGameWork.TheWizards.ServerClient.Water;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered._Engine
{
    public static class RaycastExtensions
    {
        public static T Raycast<T>(this IEnumerable<T> e, Func<T, BoundingBox> getBoundingbox, Ray ray) where T : class
        {
            var closest = new RaycastResult();
            var newResult = new RaycastResult();

            //IWorldSelectableProvider closestProvider = null;
            //Selectable closestSelectable = null;

            foreach (var s in e)
            {
                var dist = ray.xna().Intersects(getBoundingbox(s).xna());

                newResult.Set(dist,s);

                newResult = new RaycastResult();
                if (newResult.IsCloser(closest))
                {
                    newResult.CopyTo(closest);
                }

            }

            return closest.IsHit ? (T)closest.Object : null;
        }
    }
}