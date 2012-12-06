using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.CG.Spatial;
using SlimDX;

namespace MHGameWork.TheWizards.CG.Raytracing.Surfaces
{
    public class CompactGridSurface : IGenericSurface
    {
        private CompactGrid grid;
        private GridTraverser traverser;
        private BoundingBox gridBounding;

        public CompactGridSurface(CompactGrid grid)
        {

            this.grid = grid;
            traverser = new GridTraverser();
            traverser.NodeSize = grid.NodeSize;
            traverser.GridOffset = grid.GridOffset;

            var minimum = grid.GridOffset;
            var maximum = minimum + grid.NodeCount.ToVector3() * grid.NodeSize;

            gridBounding = new BoundingBox(minimum, maximum);
        }

        public void Intersects(ref RayTrace trace, out float? result, out IShadeCommand shadeCommand, bool generateShadeCommand)
        {
            float? boxDist;
            trace.Ray.Intersects(ref gridBounding, out boxDist);
            if (!boxDist.HasValue || boxDist.Value < trace.Start)
            {
                result = null;
                shadeCommand = null;
                return;
            }






            var tr = new RayTrace(trace.Ray, boxDist.Value+0.001f, trace.End);
            float? res = null;
            IShadeCommand cmd = null;
            traverser.Traverse(tr, delegate(Point3 arg)
                                          {
                                              if (!grid.InGrid(arg)) return true;
                                              var cellIndex = grid.getCellIndex(arg);
                                              var count = grid.GetNumberObjects(cellIndex);

                                              float? temp;
                                              IShadeCommand iCmd;

                                              for (int i = 0; i < count; i++)
                                              {
                                                  var surface = (IGenericSurface)grid.getCellObject(cellIndex, i);
                                                  surface.Intersects(ref tr, out temp, out iCmd,
                                                                     generateShadeCommand);

                                                  GenericTraceableScene.changeWhenCloser(ref res, ref cmd, ref temp,
                                                                                         ref iCmd);
                                                  if (!generateShadeCommand && res.HasValue)
                                                  {
                                                      //Dont wait for generating shading commands since we dont need, just return hit
                                                      return true;
                                                  }
                                              }
                                              return res.HasValue;
                                          });


            result = res;
            shadeCommand = cmd;
        }
    }
}
