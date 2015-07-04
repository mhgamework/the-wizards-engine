using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra.Single;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    public static class QEFCalculator
    {
        private static float[,] ABuffer = new float[3, 3];
        private static float[] BBuffer = new float[3];
        /// <summary>
        /// WARNING MIGHT NOT BE THREAD SAFE ANYMORE
        /// </summary>
        /// <param name="normals"></param>
        /// <param name="posses"></param>
        /// <param name="preferredPosition"></param>
        /// <returns></returns>
        public static Vector<float> CalculateCubeQEF(Vector3[] normals, Vector3[] posses, Vector3 preferredPosition)
        {
            ABuffer[0, 0] = normals[0].X;
            ABuffer[0, 1] = normals[0].Y;
            ABuffer[0, 2] = normals[0].Z;

            ABuffer[1, 0] = normals[1].X;
            ABuffer[1, 1] = normals[1].Y;
            ABuffer[1, 2] = normals[1].Z;

            ABuffer[2, 0] = normals[2].X;
            ABuffer[2, 1] = normals[2].Y;
            ABuffer[2, 2] = normals[2].Z;

            var A = DenseMatrix.OfRowArrays(normals.Select(e => new[] { e.X, e.Y, e.Z }).ToArray());
            //var A = DenseMatrix.OfArray(ABuffer); //NOT USED UNSURE IF IT IS BUGGED

            BBuffer[0] = Vector3.Dot(normals[0], posses[0] - preferredPosition);
            BBuffer[1] = Vector3.Dot(normals[1], posses[1] - preferredPosition);
            BBuffer[2] = Vector3.Dot(normals[2], posses[2] - preferredPosition);

            var b = DenseVector.OfArray(normals.Zip(posses.Select(p => p - preferredPosition), Vector3.Dot).ToArray());
            //var b = DenseVector.OfArray(BBuffer); //NOT USED UNSURE IF IT IS BUGGED
            //TODO: think about the fact that x-p is not normalized with respect to the normal vector
            var leastsquares = CalculateQEF(A, b);
            return leastsquares + DenseVector.OfArray(new[] { preferredPosition.X, preferredPosition.Y, preferredPosition.Z });
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