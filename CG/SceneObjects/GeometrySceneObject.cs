using System;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;
using MHGameWork.TheWizards.CG.Shading;

namespace MHGameWork.TheWizards.CG.SceneObjects
{
    public class GeometrySceneObject : ISceneObject
    {
        public GeometrySceneObject(IGeometricSurface geometricSurface, IShader shader)
        {
            GeometricSurface = geometricSurface;
            Shader = shader;
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
            GeometricSurface.Intersects(ref trace, ref result); //TODO: shade command

            
            result.ShadeDelegate = shade;
        }

        private Color4 shade(ref GeometryInput input,ref RayTrace trace)
        {
            return Shader.Shade(input, trace);
        }
    }
}
