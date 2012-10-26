using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Raycasting;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.CG
{
    /// <summary>
    /// Fragment tracer for triangle meshes
    /// </summary>
    public class MeshFragmentTracer : IFragmentTracer
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

        public FragmentInput TraceFragment(RayTrace rayTrace)
        {
            var result = new RaycastResult();
            var temp = new RaycastResult();

            WorldRendering.Entity rEnt = null;
            MeshRaycastResult rResult = new MeshRaycastResult();

            foreach (var ent in entities)
            {

                var transformed = rayTrace.Ray.Transform(Matrix.Invert(ent.WorldMatrix));

                if (!transformed.xna().Intersects(boxes[ent].xna()).HasValue)
                    continue;

                MeshRaycastResult meshRaycastResult;
                var dist = MeshRaycaster.RaycastMesh(ent.Mesh, transformed, out meshRaycastResult);
                if (dist < rayTrace.Start || dist > rayTrace.End) continue;

                temp.Set(dist, ent);

                if (temp.IsCloser(result))
                {
                    if (meshRaycastResult.U > 1 || meshRaycastResult.U < 0) Debugger.Break();
                    if (meshRaycastResult.V > 1 || meshRaycastResult.V < 0) Debugger.Break();
                    temp.CopyTo(result);
                    rEnt = ent;
                    rResult = meshRaycastResult;



                }
            }
            if (rEnt == null) return new FragmentInput { Clip = true };
            var ret = builder.CalculateInput(rEnt.Mesh, rEnt.WorldMatrix, rResult);
            ret.Position = result.CalculateHitPoint(rayTrace.Ray);

            return ret;

        }
    }
}
