using System;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra.Single;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    [TestFixture]
    public class DualContouringTest
    {
        [Test]
        public void Test()
        {
            var size = 3;
            var vertData = new Array3D<bool>(new Point3(size, size, size));
            var edgeData = new Array3D<Vector4>(new Point3(size, size, size));
            // Idea: dont use datastructure for now, use dictionary
            // Better define datastructure, and adjust cube_edges to it
            var cube_verts = (from x in Enumerable.Range(0, 2)
                              from y in Enumerable.Range(0, 2)
                              from z in Enumerable.Range(0, 2)
                              select new Vector3(x, y, z)).ToArray();

            var cube_edges = (from v in cube_verts
                              from offset in new[] { Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ }
                              where (v + offset).X < 1.5
                              where (v + offset).Y < 1.5
                              where (v + offset).Z < 1.5
                              select new { Start = v, End = v + offset }).Distinct().ToArray();

            Func<Vector3, Vector4[]> getEdgeData = null;

            for (int x = 0; x < size - 1; x++)
                for (int y = 0; y < size - 1; y++)
                    for (int z = 0; z < size - 1; z++)
                    {
                        var curr = new Vector3(x, y, z);
                        var signs = from offset in cube_verts
                                    select vertData[(curr + offset).ToPoint3Rounded()];
                        if (signs.All(v => v) || !signs.Any(v => v)) continue; // no sign changes

                        var data = getEdgeData(curr);
                        var posses = cube_edges.Select(e => Vector3.Lerp(e.Start, e.End, data.W));
                        var normals = data.Select(k => k.TakeXYZ());

                        var A = DenseMatrix.OfRowArrays(normals.Select(e => new[] { e.X, e.Y, e.Z }).ToArray());
                        var b = DenseVector.OfArray(normals.Zip(posses, Vector3.Dot).ToArray());

                        var leastsquares = CalculateQEF(A,b);

                    }


        }


        public Vector3 CalculateQEF(DenseMatrix denseMatrix, DenseVector denseVector)
        {
            /*// compute the SVD
            Svd svd = A.Svd(true);

            // get matrix of left singular vectors with first n columns of U
            Matrix<double> U1 = svd.U().SubMatrix(0, m, 0, n);
            // get matrix of singular values
            Matrix<double> S = newDiagonalMatrix(n, n, svd.S().ToArray());
            // get matrix of right singular vectors
            Matrix<double> V = svd.VT().Transpose();

            x = V.Multiply(S.Inverse()).Multiply(U1.Transpose().Multiply(dataY));*/
        }


    }
}