using System;
using MHGameWork.TheWizards.CG.Lighting;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;
using MHGameWork.TheWizards.CG.Shading;

namespace MHGameWork.TheWizards.CG.SceneObjects
{
    public class GeometrySceneObject : ISceneObject
    {
        public GeometrySceneObject(IGeometry geometry, IShader shader)
        {
            Geometry = geometry;
            Shader = shader;
            CastsShadows = true;

        }

        private IGeometry geometry;
        public IGeometry Geometry
        {
            get { return geometry; }
            set
            {
                geometry = value;
                bb = geometry.CalculateBoundingBox();
            }
        }

        public IShader Shader { get; set; }
        public bool CastsShadows { get; set; }

        private BoundingBox bb;

        public BoundingBox BoundingBox
        {
            get { return bb; }
        }

        public void Intersects(ref RayTrace trace, ref TraceResult result)
        {
            if (trace.IsShadowRay && !CastsShadows) return;
            Geometry.Intersects(ref trace, ref result); //TODO: shade command

            result.ShadeDelegate = shade;
        }

        private Color4 shade(ref TraceResult input, ref RayTrace trace)
        {
            return Shader.Shade(input, trace);
        }
    }
}
