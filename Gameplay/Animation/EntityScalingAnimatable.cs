using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Animation;
using SlimDX;

namespace MHGameWork.TheWizards
{
    public class EntityScalingAnimatable : IAnimatable
    {
        public WorldRendering.Entity Entity;

        public void Set(Keyframe prev, Keyframe next, float percent)
        {
            if (prev == null && next == null)
                throw new Exception("Invalid arguments given");

            if (prev != null && !(prev.Value is Vector3))
                throw new Exception("Invalid arguments given");

            if (next != null && !(next.Value is Vector3))
                throw new Exception("Invalid arguments given");

            Vector3 cTrans;
            Quaternion cRot;
            Vector3 cScale;
            Entity.WorldMatrix.Decompose(out cScale, out cRot, out cTrans);

            Vector3 newScale = new Vector3();

            if (prev == null)
            {
                newScale = (Vector3)next.Value;
            }
            else if (next == null)
            {
                newScale = (Vector3)prev.Value;
            }
            else
            {
                newScale.X = interpolate(((Vector3)prev.Value).X, ((Vector3)next.Value).X, percent);
                newScale.Y = interpolate(((Vector3)prev.Value).Y, ((Vector3)next.Value).Y, percent);
                newScale.Z = interpolate(((Vector3)prev.Value).Z, ((Vector3)next.Value).Z, percent);
            }

            Entity.WorldMatrix = Matrix.Scaling(newScale) * Matrix.RotationQuaternion(cRot) * Matrix.Translation(cTrans);
        }

        private float interpolate(float startVal, float endVal, float percent)
        {
            return startVal + percent * (endVal - startVal);
        }
    }
}
