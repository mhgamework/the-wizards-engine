using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace DirectX11
{
    public struct BoundingFrustum
    {
        public Matrix Matrix;

        public BoundingFrustum(Matrix viewProjection)
        {
            this.Matrix = viewProjection;
           

        }

        public void GetCorners(Vector3[] outCorners)
        {
            var corners = new Vector3[8];
            int i = 0;
            corners[i] = new Vector3(-1, 1, 0); i++;
            corners[i] = new Vector3(1, 1, 0); i++;
            corners[i] = new Vector3(1, -1, 0); i++;
            corners[i] = new Vector3(-1, -1, 0); i++;
            corners[i] = new Vector3(-1, 1, 1); i++;
            corners[i] = new Vector3(1, 1, 1); i++;
            corners[i] = new Vector3(1, -1, 1); i++;
            corners[i] = new Vector3(-1, -1, 1); i++;

            var mat = Matrix.Invert(Matrix);

            Vector3.TransformCoordinate(corners, ref mat, outCorners);

        }
        public Vector3[] GetCorners()
        {
            var corners = new Vector3[8];

            GetCorners(corners);

            return corners;
        }
    }
}
