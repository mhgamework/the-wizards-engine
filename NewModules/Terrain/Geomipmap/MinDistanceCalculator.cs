using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Terrain.Geomipmap
{
    public class MinDistanceCalculator
    {

        public static  float[] CalculateMinDistancesSquared(Matrix projection, HeightMap map, int blockSize, int blockNumX, int blockNumY)
        {
            var maxDetailLevel = IndexBufferBuilder.CalculateMaxDetailLevel(blockSize);
            float[] localMinDistancesSquared = new float[maxDetailLevel + 1];

            for (int i = 0; i < maxDetailLevel + 1; i++)
            {
                float minDist = CalculateLevelMinDistanceSq(i, projection, map, blockSize, blockNumX, blockNumY);
                localMinDistancesSquared[i] = minDist * minDist;
            }

            return localMinDistancesSquared;

        }

        public static float CalculateLevelMinDistanceSq(int level, Matrix projection, HeightMap map, int blockSize, int blockNumX, int blockNumY)
        {
            float error = CalculateLevelError(level, map, blockSize, blockNumX, blockNumY);

            //Willem de Boer:
            // Dn = error * C
            // C = A / T
            // A = n / |t|
            // T = ( 2 * threshold ) / verticalResolution

            //Relfection on class Matrix:
            // matrix.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            // matrix.M43 = (nearPlaneDistance * farPlaneDistance) / (nearPlaneDistance - farPlaneDistance);
            // ==>    M43 / M33 = nearPlaneDistance
            //
            // matrix.M22 = 1f / ((float) Math.Tan((double) (fieldOfView * 0.5f)));
            // matrix.M22 = (2f * nearPlaneDistance) / height;
            // (http://www.avl.iu.edu/~ewernert/b581/lectures/12.2/index.html):
            // top = near * tan(PI/180 * viewAngle/2) 
            // top = near * 1f / matrix.M22
            // top = near / M22



            float threshold = 10;
            float n = projection.M43 / projection.M33;
            float t = n / projection.M22;  // 768f / 2f;
            float verticalResolution = 768f;


            //lijkt hetzelfde te zijn als m11
            float A = n / Math.Abs(t);

            float T = (2 * threshold) / verticalResolution;

            float C = A / T;

            float Dn = error * C;



            /*float threshold = 6;


            float Dn = 0;

            Vector3 vProj1 = Vector3.Zero;
            Vector3 vProj2 = new Vector3( 0, threshold, 0 );
            Matrix inverseProj = Matrix.Invert( projection );

            Vector3 v1 = Vector3.Transform( vProj1, inverseProj );
            Vector3 v2 = Vector3.Transform( vProj2, inverseProj );

            float dist = Vector3.Distance( v1, v2 );*/







            return Dn;


        }

        public static float CalculateLevelError(int level, HeightMap map, int blockSize, int blockNumX, int blockNumY)
        {
            int stepping = 1 << level;

            float maxError = 0;

            //We go through all the quads in the selected level en interpolate a height value for every vertex that is left out


            int cx;
            int cz;
            float tl; //top left
            float tr; //top right
            float bl; //bottom left
            float br; //bottom right
            float e; //error


            //TODO: check these next 2 values
            var x = blockNumX * blockSize;
            var z = blockNumY * blockSize;

            for (int quadZ = 0; quadZ < blockSize; quadZ += stepping)
            {
                for (int quadX = 0; quadX < blockSize; quadX += stepping)
                {

                    cx = x + quadX;
                    cz = z + quadZ;
                    tl = map.GetHeight(cx, cz);
                    tr = map.GetHeight(cx + stepping, cz);
                    bl = map.GetHeight(cx, cz + stepping);
                    br = map.GetHeight(cx + stepping, cz + stepping);


                    for (int iz = 0; iz <= stepping; iz++)
                    {
                        for (int ix = 0; ix <= stepping; ix++)
                        {
                            //We could skip the corners but the error is 0 on those points anyway
                            float lerpX = MathHelper.Lerp(tl, tr, (float)ix / (float)stepping);
                            float lerpZ = MathHelper.Lerp(bl, br, (float)iz / (float)stepping);
                            float lerp = (lerpX + lerpZ) * 0.5f;

                            e = Math.Abs(map.GetHeight(cx + ix, cz + iz) - lerp);
                            maxError = MathHelper.Max(maxError, e);


                        }
                    }

                }
            }

            return maxError;

        }



    }
}
