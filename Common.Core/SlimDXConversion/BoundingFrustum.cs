using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards;
using SlimDX;

namespace DirectX11
{
    public class BoundingFrustum
    {
        private Matrix matrix;
        public Matrix Matrix
        {
            get { return matrix; }
            set { matrix = value;
                updatePlanes(value);
            }
        }

        public Plane[] Planes { get; private set; }
        public BoundingFrustum(Matrix viewProjection)
        {
            Planes = new Plane[6];

            Matrix = viewProjection;

        }

        private void updatePlanes(Matrix viewProjection)
        {
            var xnaFrustum = new Microsoft.Xna.Framework.BoundingFrustum(viewProjection.xna());

            Planes[0] = xnaFrustum.Bottom.dx();
            Planes[1] = xnaFrustum.Top.dx();
            Planes[2] = xnaFrustum.Right.dx();
            Planes[3] = xnaFrustum.Left.dx();
            Planes[4] = xnaFrustum.Near.dx();
            Planes[5] = xnaFrustum.Far.dx();
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

        public ContainmentType Contains(BoundingBox b)
        {
            // http://www.lighthouse3d.com/tutorials/view-frustum-culling/geometric-approach-testing-boxes-ii/

            var ret = ContainmentType.Contains;

            var p = new Vector3();
            var n = new Vector3();

            // p vertex = fartnest along plane normal
            // n vertex = farthest along -planeNormal

            // Return 0: outside  1: inside
            for (int i = 0; i < 6; i++)
            {
                //Note: can collapse p and n

                p.X = Planes[i].Normal.X < 0 ? b.Minimum.X : b.Maximum.X;
                p.Y = Planes[i].Normal.Y < 0 ? b.Minimum.Y : b.Maximum.Y;
                p.Z = Planes[i].Normal.Z < 0 ? b.Minimum.Z : b.Maximum.Z;

                n.X = Planes[i].Normal.X < 0 ? b.Maximum.X : b.Minimum.X;
                n.Y = Planes[i].Normal.Y < 0 ? b.Maximum.Y : b.Minimum.Y;
                n.Z = Planes[i].Normal.Z < 0 ? b.Maximum.Z : b.Minimum.Z;

                if (Vector3.Dot(n, Planes[i].Normal) > -Planes[i].D)
                    return ContainmentType.Disjoint;
                else if (Vector3.Dot(p, Planes[i].Normal) > -Planes[i].D)
                    ret = ContainmentType.Intersects;
            }
            return ret;

        }

        public static implicit operator Microsoft.Xna.Framework.BoundingFrustum(BoundingFrustum b)
        {
            return new Microsoft.Xna.Framework.BoundingFrustum(b.Matrix.xna());
        }
    }
}
