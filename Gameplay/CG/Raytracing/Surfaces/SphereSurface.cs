using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Shading;
using SlimDX;

namespace MHGameWork.TheWizards.CG.Raytracing
{
    public class SphereSurface : IGenericSurface
    {
        private IShader shader;
        private BoundingSphere sphere;
        public bool DrawsShadows = true;

        public SphereSurface(IShader shader, BoundingSphere sphere)
        {
            this.shader = shader;
            this.sphere = sphere;
        }

        public void Intersects(ref RayTrace trace, out float? result, out IShadeCommand shadeCommand, bool generateShadeCommand)
        {
            if (!DrawsShadows && ShadowsSettings.ShadowsMode)
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
            float x = sphere.Center.X - ray.Position.X;
            float y = sphere.Center.Y - ray.Position.Y;
            float z = sphere.Center.Z - ray.Position.Z;
            float length = (float)((double)x * (double)x + (double)y * (double)y + (double)z * (double)z);
            float radiusSquare = sphere.Radius * sphere.Radius;
            if ((double)length <= (double)radiusSquare && false)
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
                if (result < 0)
                    result =  dot + root;
            }
        }

        private class SphereShadeCommand : IShadeCommand
        {
            private IShader shader;
            private Vector3 hitPoint;
            private SphereSurface sphere;
            private readonly RayTrace trace;

            public SphereShadeCommand(IShader shader, Vector3 hitPoint, SphereSurface sphere, RayTrace trace)
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
