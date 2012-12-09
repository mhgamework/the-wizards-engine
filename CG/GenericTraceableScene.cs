using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Math;

namespace MHGameWork.TheWizards.CG.Raytracing
{
    /// <summary>
    /// Responsible for rendering Generic surfaces
    /// </summary>
    public class GenericTraceableScene : ITraceableScene
    {
        private List<ISceneObject> entities = new List<ISceneObject>();

        private static Color4 backgroundColor = new Color4(Color.SkyBlue);


        public void AddSceneObject(ISceneObject obj)
        {
            entities.Add(obj);
        }
        public GeometrySceneObject AddGenericSurface(IGeometricSurface geometricSurface)
        {
            var ret = new GeometrySceneObject();
            ret.GeometricSurface = geometricSurface;
            AddSceneObject(ret);

            return ret;
        }

        public void Intersect(RayTrace rayTrace, out TraceResult result)
        {
            result = new TraceResult();
            result.ShadeDelegate = () => backgroundColor;
            foreach (var ent in entities)
            {
                var newResult = new TraceResult();
                ent.Intersects(ref rayTrace, ref newResult);
                result.AddResult(ref newResult);

                if (rayTrace.FirstHit && result.IsHit)
                    break; // Return first hit
            }

        }
    }
}
