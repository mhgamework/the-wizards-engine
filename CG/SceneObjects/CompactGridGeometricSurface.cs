using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Spatial;

namespace MHGameWork.TheWizards.CG.Raytracing.Surfaces
{
    public class CompactGridGeometricSurface : ISceneObject
    {
        private CompactGrid grid;
        private GridTraverser traverser;
        private BoundingBox gridBounding;

        public CompactGridGeometricSurface(CompactGrid grid)
        {

            this.grid = grid;
            traverser = new GridTraverser();
            traverser.NodeSize = grid.NodeSize;
            traverser.GridOffset = grid.GridOffset;

            var minimum = grid.GridOffset;
            var maximum = minimum + grid.NodeCount.ToVector3() * grid.NodeSize;

            gridBounding = new BoundingBox(minimum, maximum);
        }

        public void Intersects(ref RayTrace trace, ref TraceResult result)
        {
            float? boxDist;
            trace.Ray.Intersects(ref gridBounding, out boxDist);
            if (!boxDist.HasValue || boxDist.Value < trace.Start)
                return;


            var tr = trace; // copy
            tr.Start = boxDist.Value + 0.001f;

            var closestResult = result;

            traverser.Traverse(tr, delegate(Point3 arg)
                                          {
                                              if (!grid.InGrid(arg)) return true;
                                              var cellIndex = grid.getCellIndex(arg);
                                              var count = grid.GetNumberObjects(cellIndex);

                                              for (int i = 0; i < count; i++)
                                              {
                                                  var surface = grid.getCellObject(cellIndex, i);

                                                  var newResult = new TraceResult();

                                                  surface.Intersects(ref tr, ref newResult);
                                                  closestResult.AddResult(ref newResult);

                                                  if (tr.FirstHit)
                                                      break;
                                              }
                                              return closestResult.IsHit;
                                          });
            result = closestResult;
        }

        public BoundingBox BoundingBox
        {
            get { return gridBounding; }
        }


    }
}
