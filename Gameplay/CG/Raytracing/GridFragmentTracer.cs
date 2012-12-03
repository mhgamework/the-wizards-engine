using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.CG.Spatial;
using MHGameWork.TheWizards.Raycasting;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.CG
{
    /// <summary>
    /// Fragment tracer for triangle meshes
    /// </summary>
    public class GridFragmentTracer : IFragmentTracer
    {
        public GridFragmentTracer(List<ISurface> triangles)
        {
            grid = new CompactGrid();
            grid.buildGrid(triangles);

            traverser = new GridTraverser();
        }

        private Dictionary<WorldRendering.Entity, BoundingBox> boxes =
            new Dictionary<WorldRendering.Entity, BoundingBox>();

        private WorldRaycaster worldCaster = new WorldRaycaster();

        private IMeshFragmentInputBuilder builder = new SimpleMeshFragmentInputBuilder();
        private GridTraverser traverser;
        private CompactGrid grid;

        public FragmentInput TraceFragment(RayTrace rayTrace)
        {
            traverser.Traverse(rayTrace, delegate(Point3 pos)
                                             {
                                                 int cell = grid.getCIndex(pos);
                                                 for (int i = 0; i < grid.GetNumberObjects(cell); i++)
                                                 {
                                                     var o = grid.getCellObject(cell, i);
                                                     
                                                     o.TraceFragment(rayTrace);
                                                 }
                                             });
            



            var result = new RaycastResult();
            var temp = new RaycastResult();

            WorldRendering.Entity rEnt = null;
            MeshRaycastResult rResult = new MeshRaycastResult();

            foreach (var ent in entities)
            {

                var transformed = rayTrace.Ray.Transform(Matrix.Invert(ent.WorldMatrix));
                transformed.Position += transformed.Direction*rayTrace.Start;
                if (!transformed.xna().Intersects(boxes[ent].xna()).HasValue)
                    continue;

                MeshRaycastResult meshRaycastResult;
                var dist = MeshRaycaster.RaycastMesh(ent.Mesh, transformed, out meshRaycastResult);
                if (dist > (rayTrace.End - rayTrace.Start)) continue;

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
