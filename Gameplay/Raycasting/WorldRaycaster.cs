using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.ModelContainer;
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
            var closest = new RaycastResult();
            var newResult = new RaycastResult();
            foreach (var ent in TW.Model.Objects.Where(o => o is WorldRendering.Entity).Select(o => o as WorldRendering.Entity))
            {
                raycastEntity(ent, ray, newResult);
                if (newResult.IsCloser(closest)) closest = newResult;
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


            //TODO: do course boundingbox check


            var transformed = ray.Transform(Matrix.Invert(ent.WorldMatrix));
            Vector3 v1, v2, v3;
            var distance = MeshRaycaster.RaycastMesh(ent.Mesh, transformed, out v1, out v2, out v3);


            newResult.Set(distance, ent);
            newResult.V1 = Vector3.TransformCoordinate(v1, ent.WorldMatrix);
            newResult.V2 = Vector3.TransformCoordinate(v2, ent.WorldMatrix);
            newResult.V3 = Vector3.TransformCoordinate(v3, ent.WorldMatrix);

        }
    }
}
