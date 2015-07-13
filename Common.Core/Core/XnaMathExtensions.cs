using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards
{
    [Obsolete("Use SlimDX")]
    public static class XnaMathExtensions
    {
        public static BoundingBox MergeWith(this BoundingBox box1, BoundingBox box2)
        {
            if (box1.Min == box1.Max)
                return box2;
            if (box2.Min == box2.Max)
                return box1;
            return BoundingBox.CreateMerged(box1, box2);
        }
        public static BoundingBox Transform(this BoundingBox box, Matrix mat)
        {
            Vector3[] corners = box.GetCorners();
            Vector3[] ret = new Vector3[8];
            Vector3.Transform(corners, ref mat, ret);
            return BoundingBox.CreateFromPoints(ret);

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
            var ret = CreateRotationMatrixMapDirection(Vector3.Forward, direction);


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

            var ret = Matrix.CreateFromAxisAngle(cross, (float)Math.Acos(Vector3.Dot(sourceDir, targetDir)));

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

        public static Matrix CreateMatrixFromFloatArray(float[] mat, int offset)
        {
            return new Matrix(
                mat[offset + 0], mat[offset + 1], mat[offset + 2], mat[offset + 3],
                mat[offset + 4], mat[offset + 5], mat[offset + 6], mat[offset + 7],
                mat[offset + 8], mat[offset + 9], mat[offset + 10], mat[offset + 11],
                mat[offset + 12], mat[offset + 13], mat[offset + 14], mat[offset + 15]);
        }

    }
}
