using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra.Single;

namespace MHGameWork.TheWizards.DualContouring.QEFs
{
    public class PseudoInverseQefCalculator : IQefCalculator
    {
        public Vector3 CalculateMinimizer(Vector3[] normals, Vector3[] posses,int numIntersections, Vector3 preferredPosition)
        {
            Vector3 preferredPosition1 = preferredPosition;
            var eNormals = normals.Take(numIntersections).Select(n => -n);
            var ePosses = posses.Take(numIntersections);

            var A = DenseMatrix.OfRowArrays(new List<Vector3>(eNormals).Select(e => new[] { e.X, e.Y, e.Z }).ToArray());

            var b = DenseVector.OfArray(new List<Vector3>(eNormals).Zip(new List<Vector3>(ePosses).Select(p => p - preferredPosition1), Vector3.Dot).ToArray());

            var pseudo = PseudoInverse(A);
         
            var leastSquares = pseudo.Multiply(b);

            var t = A.Multiply(leastSquares) - b;
            var error = t.DotProduct(t);
            if (error > 0.005)
                Console.WriteLine("Err: {0:0.000}", error);
            var res = leastSquares + DenseVector.OfArray(new[] { preferredPosition1.X, preferredPosition1.Y, preferredPosition1.Z });
            return new Vector3(res[0], res[1], res[2]);
        }


        /// <summary> 
        /// Moore–Penrose pseudoinverse 
        /// If A = U • Σ • VT is the singular value decomposition of A, then A† = V • Σ† • UT. 
        /// For a diagonal matrix such as Σ, we get the pseudoinverse by taking the reciprocal of each non-zero element 
        /// on the diagonal, leaving the zeros in place, and transposing the resulting matrix. 
        /// In numerical computation, only elements larger than some small tolerance are taken to be nonzero, 
        /// and the others are replaced by zeros. For example, in the MATLAB or NumPy function pinv, 
        /// the tolerance is taken to be t = ε • max(m,n) • max(Σ), where ε is the machine epsilon. (Wikipedia) 
        /// Edited by MH
        /// </summary> 
        /// <param name="M">The matrix to pseudoinverse</param> 
        /// <returns>The pseudoinverse of this Matrix</returns> 
        public static Matrix<float> PseudoInverse(Matrix<float> M)
        {
            Svd<float> D = M.Svd(true);
            var W = (Matrix<float>)D.W;
            var s = (Vector<float>)D.S;

            for (int i = 0; i < s.Count; i++)
            {
                if (s[i] < 0.1) // Tolerance suggested by dc paper (TODO: can s be negative?)
                    s[i] = 0;
                else
                    s[i] = 1 / s[i];
            }
            W.SetDiagonal(s);

            // (U * W * VT)T is equivalent with V * WT * UT 
            return (Matrix<float>)(D.U * W * D.VT).Transpose();
        }
    }
}