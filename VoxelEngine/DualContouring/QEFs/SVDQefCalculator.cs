using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra.Single;

namespace MHGameWork.TheWizards.DualContouring.QEFs
{
    /// <summary>
    /// WIP old implementation attempt using SVD directly, didnt work i think
    /// </summary>
    public class SVDQefCalculator : IQefCalculator
    {
        public Vector3 CalculateMinimizer(Vector3[] normals, Vector3[] posses,int numIntersections, Vector3 preferredPosition)
        {
            Vector3 preferredPosition1 = preferredPosition;
            var eNormals = normals.Take(numIntersections).Select(n => -n);
            var ePosses = posses.Take(numIntersections);

            var A = DenseMatrix.OfRowArrays(new List<Vector3>(eNormals).Select(e => new[] { e.X, e.Y, e.Z }).ToArray());
            //var A = DenseMatrix.OfArray(ABuffer); //NOT USED UNSURE IF IT IS BUGGED


            var b = DenseVector.OfArray(new List<Vector3>(eNormals).Zip(new List<Vector3>(ePosses).Select(p => p - preferredPosition1), Vector3.Dot).ToArray());


            var leastSquares = CalculateQEF(A, b);

            var t = A.Multiply(leastSquares) - b;
            var error = t.DotProduct(t);
            if (error > 0.005)
                Console.WriteLine("Err: {0:0.000}", error);
            var res = leastSquares + DenseVector.OfArray(new[] { preferredPosition1.X, preferredPosition1.Y, preferredPosition1.Z });
            return new Vector3(res[0], res[1], res[2]);
        }


        public static Vector<float> CalculateQEF(DenseMatrix A, DenseVector b)
        {

            //TODO: do as in dual contouring paper. Book at page 50 compact numerical methods for computers seems to talk about it:
            //  https://www.dropbox.com/s/o7icpca43t1smfm/(1990)%20Compact%20Numerical%20Methods%20for%20Computers.pdf?dl=0

            //return A.QR().Solve(b);

            //var pseudo = PseudoInverse(A);
            //return pseudo.Multiply(b);


            // compute the SVD
            /*Svd<float> svd = A.Svd(true);




            var m = A.RowCount;
            var n = A.ColumnCount;


            // get matrix of left singular vectors with first n columns of U
            Matrix<float> U1 = svd.U.SubMatrix(0, m, 0, n);
            // get matrix of singular values
            //TODO: not using absolute value to truncate!
            Matrix<float> S = DenseMatrix.CreateDiagonal(n, n, i => svd.S.Select(v => v > 0.1 ? v : 0).ToArray()[i]);
            // get matrix of right singular vectors
            Matrix<float> V = svd.VT.Transpose();

            return V.Multiply(S.Inverse()).Multiply(U1.Transpose().Multiply(b));*/
            throw new NotImplementedException();
        }
    }
}