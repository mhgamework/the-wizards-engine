using System;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards
{
    public static class SlimDXMathExtensions
    {
        public static BoundingBox MergeWith(this BoundingBox box1, BoundingBox box2)
        {
            if (box1.Minimum == box1.Maximum)
                return box2;
            if (box2.Minimum == box2.Maximum)
                return box1;
            return BoundingBox.Merge(box1, box2);
        }
        public static BoundingBox Transform(this BoundingBox box, Matrix mat)
        {
            Vector3[] corners = box.GetCorners();
            Vector3[] ret = new Vector3[8];
            Vector3.TransformCoordinate(corners, ref mat, ret);
            return BoundingBox.FromPoints(ret);

            /*Vector3 min = Vector3.Transform(box.Min, mat);
            Vector3 max = Vector3.Transform(box.Max, mat);
            return new BoundingBox(min, max);*/
        }

        public static Matrix CreateRotationMatrixFromDirectionVector(Vector3 direction)
        {
            /*var up = Vector3.Up;
            if (Vector3.Dot(direction, Vector3.Up) > 0.999f)
                up = Vector3.Right;

            Matrix ret = Matrix.CreateWorld(Vector3.Zero, direction, up);*/
            var ret = CreateRotationMatrixMapDirection(MathHelper.Forward, direction);


            return ret;
        }
        /// <summary>
        /// This creates a matrix that transforms sourceDir to targetDir
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="targetDir"></param>
        /// <returns></returns>
        public static Matrix CreateRotationMatrixMapDirection(Vector3 sourceDir, Vector3 targetDir)
        {
            if (Vector3.Dot(sourceDir, targetDir) > 0.999f)
                return Matrix.Identity;

            var cross = Vector3.Cross(sourceDir, targetDir);
            cross = Vector3.Normalize(cross);
            //XNAGame.Get().LineManager3D.AddLine(Vector3.Zero, cross, Color.Black);

            var ret = Matrix.RotationAxis(cross, (float)Math.Acos(Vector3.Dot(sourceDir, targetDir)));

            return ret;
        }

        public static float[] ToFloatArray(this Matrix mat)
        {
            return new float[] { 
                mat.M11, mat.M12, mat.M13, mat.M14,
                mat.M21, mat.M22, mat.M23, mat.M24,
                mat.M31, mat.M32, mat.M33, mat.M34,
                mat.M41, mat.M42, mat.M43, mat.M44
                };
        }


        public static Ray Transform(this Ray ray, Matrix transformation)
        {
            return new Ray(Vector3.TransformCoordinate(ray.Position, transformation), Vector3.TransformNormal(ray.Direction, transformation));
        }
    }
}
