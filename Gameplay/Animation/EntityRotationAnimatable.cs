using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Animation
{
    public class EntityRotationAnimatable : IAnimatable
    {
        public Engine.WorldRendering.Entity Entity;

        public void Set(Keyframe prev, Keyframe next, float percent)
        {
            if (prev == null && next == null)
                throw new Exception("Invalid arguments given");

            if (prev != null && !(prev.Value is Quaternion))
                throw new Exception("Invalid arguments given");

            if (next != null && !(next.Value is Quaternion))
                throw new Exception("Invalid arguments given");

            Vector3 cTrans;
            Quaternion cRot;
            Vector3 cScale;
            Entity.WorldMatrix.Decompose(out cScale, out cRot, out cTrans);

            var newRot = new Quaternion();

            if (prev == null)
            {
                newRot = (Quaternion)next.Value;
            }
            else if (next == null)
            {
                newRot = (Quaternion)prev.Value;
            }
            else
            {
                newRot = Quaternion.Slerp((Quaternion)prev.Value, (Quaternion)next.Value, percent);
            }

            Entity.WorldMatrix = Matrix.Scaling(cScale) * Matrix.RotationQuaternion(newRot) * Matrix.Translation(cTrans);
        }
    }
}
