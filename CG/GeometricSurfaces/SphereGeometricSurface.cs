using System;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Shading;

namespace MHGameWork.TheWizards.CG.Raytracing.Surfaces
{
    public class SphereGeometricSurface : IGeometricSurface
    {
        private IShader shader;
        private readonly float radius;
        public bool DrawsShadows = true;

        public SphereGeometricSurface(IShader shader, float radius)
        {
            this.shader = shader;
            this.radius = radius;
        }

        public BoundingBox CalculateBoundingBox()
        {
            return new BoundingBox(Vector3.One * -radius, Vector3.One * radius);
        }

        public void Intersects(ref RayTrace trace, out float? result, out IShadeCommand shadeCommand, bool generateShadeCommand)
        {
            if (!DrawsShadows && trace.IsShadowRay)
            {
                result = null;
                shadeCommand = null;
                return;
            }
            IntersectsSphere(ref trace.Ray, out result);
            trace.SetNullWhenNotInRange(ref result);

            if (!generateShadeCommand || !result.HasValue)
            {
                shadeCommand = null;
                return;
            }
            shadeCommand = new SphereShadeCommand(shader, trace.Ray.Position + trace.Ray.Direction * result.Value, this, trace);
            //shadeCommand = new SolidShadeCommand();

        }

        public void IntersectsSphere(ref Ray ray, out float? result)
        {
            float x = -ray.Position.X;
            float y = -ray.Position.Y;
            float z = -ray.Position.Z;
            float length = (float)((double)x * (double)x + (double)y * (double)y + (double)z * (double)z);
            float radiusSquare = radius * radius;
            if (false && (double)length <= (double)radiusSquare)
            {
                result = new float?(0.0f);
            }
            else
            {
                result = new float?();
                float dot = (float)((double)x * (double)ray.Direction.X + (double)y * (double)ray.Direction.Y + (double)z * (double)ray.Direction.Z);
                /*if ((double)dot < 0.0)
                    return;*/
                float diff = length - dot * dot;
                if ((double)diff > (double)radiusSquare)
                    return;
                float root = (float)System.Math.Sqrt((double)radiusSquare - (double)diff);
                result = new float?(dot - root);
                if (result < 0.001)
                    result = dot + root;
            }
        }

        private class SphereShadeCommand : IShadeCommand
        {
            private IShader shader;
            private Vector3 hitPoint;
            private SphereGeometricSurface sphere;
            private readonly RayTrace trace;

            public SphereShadeCommand(IShader shader, Vector3 hitPoint, SphereGeometricSurface sphere, RayTrace trace)
            {
                this.shader = shader;
                this.hitPoint = hitPoint;
                this.sphere = sphere;
                this.trace = trace;
            }


            public Color4 CalculateColor()
            {
                var input = new GeometryInput();
                input.Position = hitPoint;
                input.Normal = Vector3.Normalize(hitPoint - sphere.sphere.Center);

                return shader.Shade(input, trace);
            }
        }
    }
}
