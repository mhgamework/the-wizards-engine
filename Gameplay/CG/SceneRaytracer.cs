using System.Collections.Generic;
using MHGameWork.TheWizards.Raycasting;
using SlimDX;

namespace MHGameWork.TheWizards.CG
{
    public class SceneRaytracer
    {
        private List<WorldRendering.Entity> entities = new List<WorldRendering.Entity>();

        private WorldRaycaster worldCaster = new WorldRaycaster();

        public void AddEntity(WorldRendering.Entity ent)
        {
            entities.Add(ent);
        }

        public RaycastResult Raycast (Ray ray)
        {
            var result = new RaycastResult();
            var temp = new RaycastResult();
            foreach (var ent in entities)
            {
                var transformed = ray.Transform(Matrix.Invert(ent.WorldMatrix));

                Vector3 vertex1, vertex2, vertex3;
                var dist = MeshRaycaster.RaycastMesh(ent.Mesh, transformed, out vertex1, out vertex2, out vertex3);

                temp.Set(dist, ent);

                if (temp.IsCloser(result)) temp.CopyTo(result);
            }
            return result;
        }

        
    }
}
