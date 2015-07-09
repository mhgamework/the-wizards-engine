using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Animation
{
    /// <summary>
    /// Important note: X is forward axis in joints.
    /// </summary>
    public class Joint
    {
        //Data
        public string Name;
        public float Length;
        public Joint Parent;


        // Run-Time data
        /// <summary>
        /// This is the transformation of the bone from its base position(the end of the parent bone)
        /// </summary>
        public Matrix RelativeMatrix = Matrix.Identity;
        public Matrix AbsoluteMatrix = Matrix.Identity;

        public void CalculateAbsoluteMatrix()
        {
            Joint j = this;
            if (j.Parent == null)
            {
                j.AbsoluteMatrix = //Matrix.CreateRotationZ(MathHelper.PiOver2)*  // This makes X the forward direction
                                    j.RelativeMatrix;
                return;
            }

            j.AbsoluteMatrix = j.RelativeMatrix * j.Parent.AbsoluteMatrix;
            var l = j.AbsoluteMatrix;
            /*if (Math.Abs((l.Forward - Vector3.Forward).LengthSquared()) > 0.001f)
                throw new InvalidOperationException();*/
        }


        public void CalculateInitialRelativeMatrix(Matrix initialRotation)
        {
            if (Parent == null)
            {
                RelativeMatrix = initialRotation;
                return;
            }
            RelativeMatrix = initialRotation * Matrix.Translation(Parent.Length, 0, 0);
        }
    }
}
