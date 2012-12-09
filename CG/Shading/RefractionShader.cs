using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;

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
            refractiveNt = 1.01f;
            //refractiveNt = 2.0f;
            refractiveNtInverse = 1 / refractiveNt;
            r0 = calculateR0(refractiveNt);

        }



        public Color4 Shade(TraceResult f, RayTrace trace)
        {

            float oldContribution = trace.contribution;

            trace.recurseDepth++;
            var ret = shadeInternal(f, trace);
            trace.recurseDepth--;
            trace.contribution = oldContribution;

            return ret;
        }
        private Color4 shadeInternal(TraceResult f, RayTrace trace)
        {
            if (trace.recurseDepth > 30)
                return new Color4();
            if (trace.contribution < 0.01f)
                return new Color4();


            var hit = trace.Ray.Position + f.Distance.Value*trace.Ray.Direction;

            var d = trace.Ray.Direction;
            var n = f.Normal;

            Color4 k;
            Color4 a = new Color4((float)System.Math.Log(2.8f), (float)System.Math.Log(2f), (float)System.Math.Log(3f));
            TraceResult result;

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
                var smallt = ((trace.Ray.Position + trace.Ray.Direction * trace.Start) - hit).Length();

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
                    trace.Ray = new Ray(hit, r);
                    trace.Start = 0.001f;
                    trace.End = float.PositiveInfinity;
                    //TODO: trace.contribution = oldContrib * (1 - R);


                    scene.Intersect(trace, out result); //smallt?
                    return Color4.Modulate(k, result.Shade(ref trace));
                }
            }

            var oneMinusC = 1 - c;
            var R = r0 + (1 - r0) * oneMinusC * oneMinusC * oneMinusC * oneMinusC * oneMinusC;

            float oldContrib = trace.contribution;

            Color4 ret = new Color4();
            trace.Ray = new Ray(hit, r);
            trace.Start = 0.001f;
            trace.End = float.PositiveInfinity;
            trace.contribution = oldContrib * R;
            scene.Intersect(trace, out result); //smallt?


            ret += R * result.Shade(ref trace);

            trace.Ray = new Ray(hit, t);
            trace.Start = 0.001f;
            trace.End = float.PositiveInfinity;
            trace.contribution = oldContrib * (1 - R);
            scene.Intersect(trace, out result);//smallt?

            ret += (1 - R) * result.Shade(ref trace);

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
