using System;
using DirectX11;
using MHGameWork.TheWizards.IO;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.ServerClient;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    /// <summary>
    /// Hermite grid that gets its data from a density function
    /// Idea: make class density function
    /// </summary>
    public class DensityFunctionHermiteGrid:AbstractHermiteGrid
    {
        private readonly Func<Vector3, float> densityFunction;
        private readonly Point3 dimensions;
        private Func<Vector3, DCVoxelMaterial> getMaterial;

        public DensityFunctionHermiteGrid( Func<Vector3, float> densityFunction, Point3 dimensions, Func<Vector3, DCVoxelMaterial> getMaterial )
        {
            this.densityFunction = densityFunction;
            this.dimensions = dimensions;
            this.getMaterial = getMaterial;
        }

        public DensityFunctionHermiteGrid(Func<Vector3, float> densityFunction, Point3 dimensions)
        {
            this.densityFunction = densityFunction;
            this.dimensions = dimensions;
            
            var mat = new DCVoxelMaterial() {Texture = DCFiles.UVCheckerMap10_512};
            getMaterial = p => mat;
        }

        public override bool GetSign(Point3 p)
        {
            return densityFunction(p) > 0;
        }

        public override Point3 Dimensions
        {
            get { return dimensions; }
        }

        public override Vector4 getEdgeData(Point3 p, int i)
        {
            var startPos = GetEdgeOffsets(i)[0] + p;
            var endPos = GetEdgeOffsets(i)[1] + p;
            // search intersection point
            var zeroFactor = FindZeroOnLineSubdivide(densityFunction, startPos, endPos, 8);
            var point = Vector3.Lerp(startPos, endPos, zeroFactor);
            //var zeroPos = FindZeroOnLineLinearApprox(densityFunction, startPos, endPos);

            //TODO: calculate normal
            //Note: upvoid uses analytical differentiation, im going approx style
            var normal = new Vector3();
            float stepSize = 0.01f;
            normal.X = densityFunction(point + Vector3.UnitX * stepSize) - densityFunction(point - Vector3.UnitX * stepSize);
            normal.Y = densityFunction(point + Vector3.UnitY * stepSize) - densityFunction(point - Vector3.UnitY * stepSize);
            normal.Z = densityFunction(point + Vector3.UnitZ * stepSize) - densityFunction(point - Vector3.UnitZ * stepSize);

            normal.Normalize();
            normal = -normal; // This is since we want it to point to the negative side of the density field, eg air
            return new Vector4(normal, zeroFactor);
        }

        public override DCVoxelMaterial GetMaterial( Point3 cube )
        {
            return getMaterial( cube );
        }

        /// <summary>
        /// Divide line between startpos and endpos into 'divide' pieces, and linearly approx the zeros in each piece, returning the first zero found
        /// TODO: Possible problems: multiple roots can give strange results, as a the first root is always picked. 
        ///         Also, the linear approx can be very much off in the 1/divide subpiece, so we have somewhat of a limited 1/8 precision, even for simple functions
        /// TODO: probably use a better root finder, since when i have a simple function which is not linear it provides only results up to 1/8th precision.
        /// </summary>
        /// <param name="densityFunction"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="divide"></param>
        /// <returns></returns>
        public static float FindZeroOnLineSubdivide(Func<Vector3, float> densityFunction, Point3 startPos, Point3 endPos, int divide)
        {
            var factorStep = 1f / divide;
            var diff = (endPos - startPos).ToVector3();


            for (int i = 0; i < divide; i++)
            {
                var x1 = i * factorStep;
                // If this is not exactly 1 in the end we might miss skip the exact endpoint in all calculation, but this is probably not a problem since its
                //    already an approximation
                var x2 = (i + 1) * factorStep;
                var y1 = densityFunction(startPos + x1 * diff);
                var y2 = densityFunction(startPos + x2 * diff);
                if (y1 * y2 > 0) continue; // no linear zero

                // return first zero
                return FindZeroLinear(x1, x2, y1, y2);
            }
            throw new InvalidOperationException("No zero! Should not be possible since this function should only be called if start and end have a sign difference");
        }

        /// <summary>
        /// Find the x for which y = zero assuming function is linear between p1 and p2
        /// </summary>
        /// <param name="function"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static float FindZeroLinear(float x1, float x2, float y1, float y2)
        {
            // Assume linear
            var slope = (y2 - y1) / (x2 - x1);
            if (slope < 0.001) return (x1 + x2) / 2; // Return middle when slope is almost zero
            var zero = -y1 / slope + x1;
            return zero;
        }


        /// <summary>
        /// Untested
        /// </summary>
        /// <param name="densityFunction"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        private static void FindZeroOnLineLinearApprox(Func<Vector3, float> densityFunction, Point3 startPos, Point3 endPos)
        {
            var searchStart = 0f;
            var searchEnd = 1f;
            var startDens = densityFunction(Vector3.Lerp(startPos, endPos, searchStart));
            var endDens = densityFunction(Vector3.Lerp(startPos, endPos, searchEnd));
            if (startDens * endDens > 0)
                throw new InvalidOperationException("No intersection OR linear estimation doesnt hold");
            //Idea maybe take center


            for (int j = 0; j < 5; j++)
            {
                //TODO: test interstection

                // Assume linear
                var slope = (endDens - startDens) / (searchEnd - searchStart);
                // startDens + (zeroEstimate - searchStart) * slope = 0
                var zero = -startDens / slope + searchStart;
                var zeroDens = densityFunction(Vector3.Lerp(startPos, endPos, zero));

                if (startDens * zeroDens < 0)
                {
                    // Sign difference, so take start to zero
                    searchEnd = zero;
                    endDens = zeroDens;
                }
                else if (endDens * zeroDens < 0)
                {
                    searchStart = zero;
                    startDens = zeroDens;
                }
                else
                {
                    // No sign difference??
                    throw new InvalidOperationException("No intersection OR linear estimation doesnt hold");
                }
            }
        }


        public HermiteDataGrid ToDataGrid()
        {
            return null;
        }
    }
}