using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Raytracing;
using SlimDX;

namespace MHGameWork.TheWizards.CG.Shading
{
    public class RefractionShader : IShader
    {
        private ITraceableScene scene;
        private float r0;
        private float refractiveN;
        private float refractiveNt;
        private float refractiveNtInverse;

        public RefractionShader(ITraceableScene scene)
        {
            this.scene = scene;
            refractiveN = 1;
            refractiveNt = 1.0f;
            refractiveNtInverse = 1 / refractiveNt;
            r0 = calculateR0(refractiveNt);

        }

        public static int recurseDepth = 0;

        public static float contribution = 1;

        public Color4 Shade(GeometryInput f, RayTrace trace)
        {
            float oldContribution = contribution;
            
            recurseDepth++;
            var ret = shadeInternal(f, trace);
            recurseDepth--;
            contribution = oldContribution;

            return ret;
        }
        private Color4 shadeInternal(GeometryInput f, RayTrace trace)
        {
            if (recurseDepth > 30)
                return new Color4();
            if (contribution < 0.01f)
                return new Color4();

            var d = trace.Ray.Direction;
            var n = f.Normal;

            Color4 k;
            Color4 a = new Color4((float)System.Math.Log(2f), (float)System.Math.Log(1f), (float)System.Math.Log(2f));
            IShadeCommand cmd;

            Vector3 t;
            var r = Vector3.Reflect(d, n);
            var c = -Vector3.Dot(d, n);
            if (Vector3.Dot(d, n) < 0.0001)
            {
                refract(d, n, refractiveNtInverse, out t); // Note this is optimized, the inverse is given!!
                k = new Color4(1, 1, 1, 1);
            }
            else
            {
                var smallt = ((trace.Ray.Position + trace.Ray.Direction * trace.Start) - f.Position).Length();

                k.Red = (float)System.Math.Exp(-a.Red * smallt);
                k.Green = (float)System.Math.Exp(-a.Green * smallt);
                k.Blue = (float)System.Math.Exp(-a.Blue * smallt);
                k.Alpha = 1;
                if (refract(d, -n, refractiveNt, out t)) // Note this is optimized, the inverse is given!!
                {
                    c = Vector3.Dot(t, n);
                }
                else
                {
                    scene.Intersect(new RayTrace(new Ray(f.Position, r), 0.001f, float.PositiveInfinity), out cmd, true); //smallt?
                    return Color4.Modulate(k, cmd.CalculateColor());
                }
            }

            var oneMinusC = 1 - c;
            var R = r0 + (1 - r0) * oneMinusC * oneMinusC * oneMinusC * oneMinusC * oneMinusC;

            Color4 ret = new Color4();

            scene.Intersect(new RayTrace(new Ray(f.Position, r), 0.001f, float.PositiveInfinity), out cmd, true); //smallt?

            float oldContrib = contribution;
            contribution =oldContrib* R;

            ret += R * cmd.CalculateColor();

            scene.Intersect(new RayTrace(new Ray(f.Position, t), 0.001f, float.PositiveInfinity), out cmd, true);//smallt?
            contribution = oldContrib * (1-R);
            ret += (1 - R) * cmd.CalculateColor();

            return Color4.Modulate(k, ret);

        }

        private float calculateR0(float refractiveN)
        {
            return (refractiveN - 1) * (refractiveN - 1) / (refractiveN + 1) * (refractiveN + 1);
        }

        private bool refract(Vector3 direction, Vector3 normal, float ntInverse, out Vector3 transmission)
        {


            double dot = Vector3.Dot(direction, normal);

            var underRoot = 1 - refractiveN * refractiveN * (1 - dot * dot) * ntInverse * ntInverse;

            if (underRoot < 0.0001)
            {
                transmission = new Vector3();
                return false;
            }

            transmission = refractiveN * (direction - normal * (float)dot) * (float)ntInverse - normal * (float)System.Math.Sqrt(underRoot);
            return true;
        }
    }
}
