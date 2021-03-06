using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Graphics
{
    public class Curve3D
    {
        private Curve curveX;
        private Curve curveY;
        private Curve curveZ;

        public CurveLoopType PreLoop
        {
            get
            {
                return curveX.PreLoop;
            }
            set
            {
                curveX.PreLoop = value;
                curveY.PreLoop = value;
                curveZ.PreLoop = value;

            }
        }

        /// <summary>
        /// Hack for supporting the render method in the test
        /// </summary>
        public float GetStart()
        {
            return curveX.Keys[0].Position;
        }
        /// <summary>
        /// Hack for supporting the render method in the test
        /// </summary>
        public float GetEnd()
        {
            return curveX.Keys[curveX.Keys.Count - 1].Position;
        }

        public CurveLoopType PostLoop
        {
            get
            {
                return curveX.PostLoop;
            }
            set
            {
                curveX.PostLoop = value;
                curveY.PostLoop = value;
                curveZ.PostLoop = value;

            }
        }

        public Curve3D()
        {
            curveX = new Curve();
            curveY = new Curve();
            curveZ = new Curve();
        }

        public void AddKey(float position, Vector3 value)
        {
            curveX.Keys.Add(new CurveKey(position, value.X));
            curveY.Keys.Add(new CurveKey(position, value.Y));
            curveZ.Keys.Add(new CurveKey(position, value.Z));

        }

        public void SetTangents()
        {
            CurveKey prev;
            CurveKey current;
            CurveKey next;
            int prevIndex;
            int nextIndex;
            for (int i = 0; i < curveX.Keys.Count; i++)
            {
                prevIndex = i - 1;
                if (prevIndex < 0) prevIndex = i;

                nextIndex = i + 1;
                if (nextIndex == curveX.Keys.Count) nextIndex = i;

                prev = curveX.Keys[prevIndex];
                next = curveX.Keys[nextIndex];
                current = curveX.Keys[i];
                SetCurveKeyTangent(ref prev, ref current, ref next);
                curveX.Keys[i] = current;
                prev = curveY.Keys[prevIndex];
                next = curveY.Keys[nextIndex];
                current = curveY.Keys[i];
                SetCurveKeyTangent(ref prev, ref current, ref next);
                curveY.Keys[i] = current;

                prev = curveZ.Keys[prevIndex];
                next = curveZ.Keys[nextIndex];
                current = curveZ.Keys[i];
                SetCurveKeyTangent(ref prev, ref current, ref next);
                curveZ.Keys[i] = current;
            }
        }
        static void SetCurveKeyTangent(ref CurveKey prev, ref CurveKey cur,
            ref CurveKey next)
        {
            float dt = next.Position - prev.Position;
            float dv = next.Value - prev.Value;
            if (Math.Abs(dv) < Single.Epsilon)
            {
                cur.TangentIn = 0;
                cur.TangentOut = 0;
            }
            else
            {
                // The in and out tangents should be equal to the 
                // slope between the adjacent keys.
                cur.TangentIn = dv * (cur.Position - prev.Position) / dt;
                cur.TangentOut = dv * (next.Position - cur.Position) / dt;
            }
        }


        public Vector3 Evaluate(float position)
        {
            return new Vector3(
                curveX.Evaluate(position),
                curveY.Evaluate(position),
                curveZ.Evaluate(position));
        }

        public BoundingSphere CalculateBoundingSphere()
        {
            Vector3[] points = new Vector3[curveX.Keys.Count];
            for (int i = 0; i < curveX.Keys.Count; i++)
            {
                points[i] = new Vector3(
                    curveX.Keys[i].Value,
                    curveY.Keys[i].Value,
                    curveZ.Keys[i].Value);
            }
            return BoundingSphere.CreateFromPoints(points);
        }

        public static Curve3D CreateTestCurve()
        {
            Curve3D curve = new Curve3D();

            curve.PreLoop = CurveLoopType.Constant;
            curve.PostLoop = CurveLoopType.Cycle;

            curve.AddKey(0, new Vector3(2, 2, 2));
            curve.AddKey(1, new Vector3(4, 0, 2));
            curve.AddKey(5, new Vector3(8, 0, 7));
            curve.AddKey(7, new Vector3(2, 1, 5));
            curve.AddKey(9, new Vector3(2, 2, 2));

            curve.SetTangents();
            return curve;
        }
    }
}
