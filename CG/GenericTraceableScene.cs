using System.Collections.Generic;
using System.Drawing;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;

namespace MHGameWork.TheWizards.CG
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
        public void Intersect(RayTrace rayTrace, out TraceResult result)
        {
            result = new TraceResult();
            result.ShadeDelegate = delegate { return backgroundColor; };
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
