using System;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Shading;

namespace MHGameWork.TheWizards.CG.Raytracing
{
    public class GeometrySceneObject : ISceneObject
    {
        public GeometrySceneObject()
        {
            CastsShadows = true;
        }
        public IGeometricSurface GeometricSurface { get; set; }
        public IShader Shader { get; set; }
        public bool CastsShadows { get; set; }

        public BoundingBox BoundingBox
        {
            get { throw new NotImplementedException(); }
        }

        public void Intersects(ref RayTrace trace, ref TraceResult result)
        {
            if (trace.IsShadowRay && !CastsShadows) return;

            IShadeCommand cmd;
            GeometricSurface.Intersects(ref trace, out result.Distance, out cmd, true); //TODO: shade command

            result.ShadeDelegate = () => cmd.CalculateColor();
        }

    }
}
