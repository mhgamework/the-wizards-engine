using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Shading;

namespace MHGameWork.TheWizards.CG.Raytracing.Surfaces
{
    public class PlaneSurface : IGenericSurface
    {
        private IShader shader;
        private Plane plane;

        public PlaneSurface(IShader shader, Plane plane)
        {
            this.shader = shader;
            this.plane = plane;
        }

        public void Intersects(ref RayTrace trace, out float? result, out IShadeCommand shadeCommand, bool generateShadeCommand)
        {
            IntersectsPlane(ref trace.Ray, out result);
            trace.SetNullWhenNotInRange(ref result);

            if (!generateShadeCommand || !result.HasValue)
            {
                shadeCommand = null;
                return;
            }
            shadeCommand = new PlaneShadeCommand(shader, trace.Ray.Position + trace.Ray.Direction * result.Value, this, trace);
            //shadeCommand = new SolidShadeCommand();

        }

        //TODO:
        public void IntersectsPlane(ref Ray ray, out float? result)
        {
            float num1 = (float)((double)plane.Normal.X * (double)ray.Direction.X + (double)plane.Normal.Y * (double)ray.Direction.Y + (double)plane.Normal.Z * (double)ray.Direction.Z);
            if ((double)System.Math.Abs(num1) < 9.99999974737875E-06)
            {
                result = new float?();
            }
            else
            {
                float num2 = (float)((double)plane.Normal.X * (double)ray.Position.X + (double)plane.Normal.Y * (double)ray.Position.Y + (double)plane.Normal.Z * (double)ray.Position.Z);
                float num3 = (-plane.D - num2) / num1;
                if ((double)num3 < 0.0)
                {
                    if ((double)num3 < -9.99999974737875E-06)
                    {
                        result = new float?();
                        return;
                    }
                    else
                        result = new float?(0.0f);
                }
                result = new float?(num3);
            }
        }

        private class PlaneShadeCommand : IShadeCommand
        {
            private IShader shader;
            private Vector3 hitPoint;
            private PlaneSurface surface;
            private readonly RayTrace trace;

            public PlaneShadeCommand(IShader shader, Vector3 hitPoint, PlaneSurface surface, RayTrace trace)
            {
                this.shader = shader;
                this.hitPoint = hitPoint;
                this.surface = surface;
                this.trace = trace;
            }


            public Color4 CalculateColor()
            {
                var input = new GeometryInput();


                input.Position = hitPoint;
                input.Normal = surface.plane.Normal;

                return shader.Shade(input, trace);
            }
        }
    }
}
