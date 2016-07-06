using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra.Single;

namespace MHGameWork.TheWizards.DualContouring.QEFs
{
    /// <summary>
    /// Work in progress implementation, contains some ideas that i haven't gotten working yet
    /// </summary>
    public class WipQefCalculator : IQefCalculator
    {
        public Vector3 CalculateMinimizer(Vector3[] normals, Vector3[] posses,int numIntersections, Vector3 preferredPosition)
        {
            var res = CalculateCubeQEFNormalEquations(normals, posses, numIntersections, preferredPosition);
            return new Vector3(res[0], res[1], res[2]);
        }

        public static Vector<float> CalculateCubeQEFNormalEquations(Vector3[] normals, Vector3[] posses, int numIntersections, Vector3 preferredPosition)
        {

            var eNormals = normals.Take(numIntersections).Select(n => -n);
            var ePosses = posses.Take(numIntersections);

            var A = DenseMatrix.OfRowArrays(new List<Vector3>(eNormals).Select(e => new[] { e.X, e.Y, e.Z }).ToArray());
            //var A = DenseMatrix.OfArray(ABuffer); //NOT USED UNSURE IF IT IS BUGGED


            var b = DenseVector.OfArray(new List<Vector3>(eNormals).Zip(new List<Vector3>(ePosses).Select(p => p - preferredPosition), Vector3.Dot).ToArray());


            //var leastSquares=A.TransposeThisAndMultiply( A ).Cholesky().Solve( A.TransposeThisAndMultiply( b ) );
            //var leastSquares = A.TransposeThisAndMultiply( A ).Inverse()*A.TransposeThisAndMultiply( b );

            // From my algebra book this should be correct, however it is not numerically stable for some reason?
            // For example: all normals to the same value gives all NAN's in this method
            //var leastSquares = A.QR().Solve(b);

            var leastSquares = CalculateQEF(A, b);

            var t = A.Multiply(leastSquares) - b;
            var error = t.DotProduct(t);
            if (error > 0.005)
                Console.WriteLine("Err: {0:0.000}", error);

            return leastSquares + DenseVector.OfArray(new[] { preferredPosition.X, preferredPosition.Y, preferredPosition.Z });
        }

        /// <summary>
        /// WARNING MIGHT NOT BE THREAD SAFE
        /// </summary>
        /// <param name="normals"></param>
        /// <param name="posses"></param>
        /// <param name="preferredPosition"></param>
        /// <returns></returns>
        public static Vector<float> CalculateCubeQEFMHStyle(Vector3[] normals, Vector3[] posses, int numIntersections, Vector3 preferredPosition)
        {
            List<Vector3> normals1 = new List<Vector3>(normals.Take(numIntersections));
            List<Vector3> posses1 = new List<Vector3>(posses.Take(numIntersections));
            Vector3 preferredPosition1 = preferredPosition;
            var A = DenseMatrix.OfRowArrays(normals1.Select(e => new[] { e.X, e.Y, e.Z }).ToArray());
            //var A = DenseMatrix.OfArray(ABuffer); //NOT USED UNSURE IF IT IS BUGGED

            var b = DenseVector.OfArray(normals1.Zip(posses1.Select(p => p - preferredPosition1), Vector3.Dot).ToArray());
            //var b = DenseVector.OfArray(BBuffer); //NOT USED UNSURE IF IT IS BUGGED
            //TODO: think about the fact that x-p is not normalized with respect to the normal vector
            var leastsquares = CalculateQEF(A, b);
            return leastsquares + DenseVector.OfArray(new[] { preferredPosition1.X, preferredPosition1.Y, preferredPosition1.Z });
        }

        public static Vector<float> CalculateQEF(DenseMatrix A, DenseVector b)
        {

            //TODO: do as in dual contouring paper. Book at page 50 compact numerical methods for computers seems to talk about it:
            //  https://www.dropbox.com/s/o7icpca43t1smfm/(1990)%20Compact%20Numerical%20Methods%20for%20Computers.pdf?dl=0

            //return A.QR().Solve(b);

            var pseudo = PseudoInverse(A);
            return pseudo.Multiply(b);


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
        }


        /// <summary>
        ///     WARNING MIGHT NOT BE THREAD SAFE
        /// </summary>
        /// <param name="normals"></param>
        /// <param name="posses"></param>
        /// <param name="preferredPosition"></param>
        /// <returns></returns>
        public static Vector3 CalculateCubeQEFIteratively(Vector3[] normals, Vector3[] posses,
            int numIntersections, Vector3 preferredPosition)
        {

            var curr = preferredPosition;
            var err = 0f;

            for (int j = 0; j < 1000; j++)
            {
                err = 0;
                //Debug.Log("Start");
                var meanOnplane = new Vector3();

                for (int i = 0; i < numIntersections; i++)
                {
                    // Project on plane
                    var planePos = posses[i];
                    var planeN = normals[i];
                    var relCurr = curr - planePos;
                    var offsetAlongNormal = Vector3.Dot(relCurr, planeN);
                    var onPlane = curr + offsetAlongNormal * -planeN;
                    err += (onPlane - curr).LengthSquared();
                    meanOnplane += onPlane;
                }
                if (err < 0.0001f) break;

                //Debug.Log(err);
                meanOnplane /= numIntersections;
                curr += (meanOnplane - curr) * 0.7f; // Found this number on some blogpost
                //curr = new Vector3(0,0,0);

            }
            if (err >= 0.001f)
            {
                int a = 5;
            }


            return curr;
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