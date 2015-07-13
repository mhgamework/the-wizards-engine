using System;
using MHGameWork.TheWizards.MathExtra;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Graphics.Xna.Graphics
{
    public struct Transformation
    {
        public Vector3 Translation;
        public Quaternion Rotation;
        public Vector3 Scaling;


        public Transformation(Vector3 translation)
            : this(Vector3.One, Quaternion.Identity, translation)
        {
        }
        public Transformation(Vector3 translation, Quaternion rot)
            : this(Vector3.One, rot, translation)
        {
        }
        public Transformation(Vector3 scale, Quaternion rot, Vector3 translation)
        {
            Translation = translation;
            Rotation = rot;
            Scaling = scale;
        }

        public void SetRotation(Vector3 eulerAngles)
        {
            Rotation = Quaternion.CreateFromYawPitchRoll(eulerAngles.Y, eulerAngles.X, eulerAngles.Z);
        }

        public Vector3 GetEulerAngles()
        {
            return Functions.QuatToEuler(Rotation);
        }

        public void AddRotation(Vector3 eulerAngles)
        {
            Rotation = Rotation * Quaternion.CreateFromYawPitchRoll(eulerAngles.Y, eulerAngles.X, eulerAngles.Z);
        }

        public Matrix CreateMatrix()
        {
#if DEBUG
            if (Rotation.Equals(new Quaternion(0, 0, 0, 0)))
                throw new Exception("This Transformation is not yet initialized");
            if (Scaling.Equals(Vector3.Zero))
                throw new Exception("This Transformation is not yet initialized");
#endif
            return Matrix.CreateScale(Scaling) * Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(Translation);
        }
        public static Transformation FromMatrix(Matrix m)
        {
            Transformation transform = new Transformation();
            m.Decompose(out transform.Scaling, out transform.Rotation, out transform.Translation);

            return transform;
        }

        public static Transformation Identity
        {
            get { return new Transformation(Vector3.One, Quaternion.Identity, Vector3.Zero); }
        }

        /// <summary>
        /// Does a simple = check on all the components of this transformation
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public bool EqualsExactly(Transformation t)
        {
            if (t.Scaling.X == Scaling.X
                && t.Scaling.Y == Scaling.Y
                && t.Scaling.Z == Scaling.Z

                && t.Rotation.X == Rotation.X
                && t.Rotation.Y == Rotation.Y
                && t.Rotation.Z == Rotation.Z
                && t.Rotation.W == Rotation.W

                && t.Translation.X == Translation.X
                && t.Translation.Y == Translation.Y
                && t.Translation.Z == Translation.Z) return true;

            return false;

        }
    }
}
