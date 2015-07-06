using MHGameWork.TheWizards.Engine.Raycasting;
using MHGameWork.TheWizards.Raycasting;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring.Building
{
    public class Raycaster<T>
    {
        private RaycastResult closest = new RaycastResult();
        private RaycastResult cache = new RaycastResult();

        public void AddIntersection(Ray ray, Vector3[] vertices, Matrix world, object obj)
        {
            var localRay = ray.Transform(Matrix.Invert(world));
            Vector3 v0;
            Vector3 v1;
            Vector3 v2;
            Vector3 normal;
            var dist = MeshRaycaster.RaycastMeshPart(vertices, localRay, out v0, out v1, out v2, out normal);
            if (!dist.HasValue) return;
            var point = Vector3.TransformCoordinate(localRay.GetPoint(dist.Value), world);
            cache.Set(Vector3.Distance(ray.Position, point), obj);
            if (!cache.IsCloser(closest)) return;
            closest.Set(cache.Distance, cache.Object);
            closest.V1 = Vector3.TransformCoordinate(v0, world);
            closest.V2 = Vector3.TransformCoordinate(v1, world);
            closest.V3 = Vector3.TransformCoordinate(v2, world);
            closest.HitNormal = Vector3.TransformNormal(normal, world);
        }

        public void AddIntersection(float? dist, T obj)
        {
            cache.Set(dist, obj);
            if (cache.IsCloser(closest))
                closest = cache;
        }

        public RaycastResult GetClosest()
        {
            return closest;
        }
    }
}