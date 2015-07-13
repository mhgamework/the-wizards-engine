using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Engine.Raycasting;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered._Engine
{
    /// <summary>
    /// Warning: this is a good idea, but is untested and i think this does not work.
    /// </summary>
    public static class RaycastExtensions
    {
        public static RaycastResult Raycast<T>(this IEnumerable<T> e, Func<T, BoundingBox> getLocalBoundingbox, Func<T, Matrix> getTransform, Ray ray) where T : class
        {
            var closest = new RaycastResult();
            var newResult = new RaycastResult();

            //IWorldSelectableProvider closestProvider = null;
            //Selectable closestSelectable = null;

            foreach (var s in e)
            {
                var transformation = getTransform(s);
                var localRay = ray.Transform(Matrix.Invert(transformation));
                var dist = localRay.xna().Intersects(getLocalBoundingbox(s).xna());
                if (dist != null)
                {
                    var localPoint = localRay.GetPoint(dist.Value);
                    var point = Vector3.TransformCoordinate(localPoint, transformation);
                    dist = Vector3.Distance(ray.Position, point);
                }

                newResult.Set(dist, s);

                if (newResult.IsCloser(closest))
                {
                    newResult.CopyTo(closest);
                }

            }

            return closest;
        }

        public static T Raycast<T>(this IEnumerable<T> e, Func<T, Ray, float?> intersect, Ray ray) where T : class
        {
            var closest = RaycastDetail(e, intersect, ray);
            return closest.IsHit ? (T)closest.Object : null;
        }
        public static RaycastResult RaycastDetail<T>(this IEnumerable<T> e, Func<T, Ray, float?> intersect, Ray ray) where T : class
        {
            var closest = new RaycastResult();
            var newResult = new RaycastResult();

            //IWorldSelectableProvider closestProvider = null;
            //Selectable closestSelectable = null;

            foreach (var s in e)
            {
                var dist = intersect(s, ray);

                newResult.Set(dist, s);

                if (newResult.IsCloser(closest))
                {
                    newResult.CopyTo(closest);
                }

            }

            return closest;
        }
    }
}