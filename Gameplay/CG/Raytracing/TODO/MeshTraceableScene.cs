using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.Raycasting;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.CG
{
    /// <summary>
    /// Fragment tracer for triangle meshes
    /// TODO: merge into generic traceable scene
    /// </summary>
    public class MeshTraceableScene : ITraceableScene
    {
        private List<WorldRendering.Entity> entities = new List<WorldRendering.Entity>();

        private Dictionary<WorldRendering.Entity, BoundingBox> boxes =
            new Dictionary<WorldRendering.Entity, BoundingBox>();

        private WorldRaycaster worldCaster = new WorldRaycaster();

        public void AddEntity(WorldRendering.Entity ent)
        {
            entities.Add(ent);
            boxes.Add(ent, MeshBuilder.CalculateBoundingBox(ent.Mesh));
        }
        private IMeshFragmentInputBuilder builder = new SimpleMeshFragmentInputBuilder();

        public bool Intersect(RayTrace rayTrace, out IShadeCommand command, bool generateShadeCommand)
        {
            command = null;
            var result = new RaycastResult();
            var temp = new RaycastResult();

            WorldRendering.Entity rEnt = null;
            MeshRaycastResult rResult = new MeshRaycastResult();

            foreach (var ent in entities)
            {

                var transformed = rayTrace.Ray.Transform(Matrix.Invert(ent.WorldMatrix));
                transformed.Position += transformed.Direction * rayTrace.Start;
                if (!transformed.xna().Intersects(boxes[ent].xna()).HasValue)
                    continue;

                MeshRaycastResult meshRaycastResult;
                var dist = MeshRaycaster.RaycastMesh(ent.Mesh, transformed, out meshRaycastResult);
                if (dist > (rayTrace.End - rayTrace.Start)) continue;

                temp.Set(dist, ent);

                if (temp.IsCloser(result))
                {
                    temp.CopyTo(result);
                    rEnt = ent;
                    rResult = meshRaycastResult;
                }
            }
            if (rEnt == null) return false; //new FragmentInput { Clip = true };
            var ret = builder.CalculateInput(rEnt.Mesh, rEnt.WorldMatrix, rResult);
            ret.Position = result.CalculateHitPoint(rayTrace.Ray);

            //return ret;

            return true;
        }
    }
}
