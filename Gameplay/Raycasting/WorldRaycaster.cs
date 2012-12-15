using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards.Raycasting
{
    /// <summary>
    /// Responsible for raycasting the GamePlay world
    /// TODO: probably attach this to a scene, or move this to the scene.
    /// </summary>
    public class WorldRaycaster
    {
        public bool RaycastInvisible { get; set; }

        public RaycastResult Raycast(Ray ray)
        {
            return Raycast(ray, o => true);
        }

        public RaycastResult Raycast(Ray ray, Func<object, bool> filter)
        {
            var closest = new RaycastResult();
            var newResult = new RaycastResult();
            foreach (var ent in TW.Data.Objects.Where(o => o is WorldRendering.Entity).Select(o => o as WorldRendering.Entity))
            {
                if (!filter(ent)) continue;

                raycastEntity(ent, ray, newResult);
                if (newResult.IsCloser(closest)) newResult.CopyTo(closest);
            }

            return closest;
        }

        private void raycastEntity(WorldRendering.Entity ent, Ray ray, RaycastResult newResult)
        {
            bool abort = false;
            if (!ent.Visible && !RaycastInvisible) abort = true;
            if (ent.Mesh == null) abort = true;
            if (abort)
            {
                newResult.Set(null, ent);
                return;
            }

            var transformed = ray.Transform(Matrix.Invert(ent.WorldMatrix));


            //TODO: do course boundingbox check
            var bb = TW.Assets.GetBoundingBox(ent.Mesh);
            if (!transformed.xna().Intersects(bb.xna()).HasValue)
            {
                newResult.Set(null, ent);
                return;
            }



            Vector3 v1, v2, v3;
            var distance = MeshRaycaster.RaycastMesh(ent.Mesh, transformed, out v1, out v2, out v3);


            newResult.Set(distance, ent);
            newResult.V1 = Vector3.TransformCoordinate(v1, ent.WorldMatrix);
            newResult.V2 = Vector3.TransformCoordinate(v2, ent.WorldMatrix);
            newResult.V3 = Vector3.TransformCoordinate(v3, ent.WorldMatrix);

        }
    }
}
