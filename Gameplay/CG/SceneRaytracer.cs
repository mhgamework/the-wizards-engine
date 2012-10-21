using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.Raycasting;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;

namespace ComputerGraphics
{
    public class SceneRaytracer
    {
        private List<Entity> entities = new List<Entity>();

        private WorldRaycaster worldCaster = new WorldRaycaster();

        public void AddEntity(Entity ent)
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

        public FragmentInput TraceFragment(Ray ray, float min, float max, RaycastResult result)
        {
            var ret = new FragmentInput();
            ret.Position = result.CalculateHitPoint(ray);
            ret.Diffuse = new Color4(1, 0, 0);
            ret.Normal = new Vector3(0, 0, 1);

            return ret;
        }
    }
}
