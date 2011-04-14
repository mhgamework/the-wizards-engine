using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Water
{
    public static class Utils
    {
        public class BoundingBox
        {
            public Vector3 min;
            public Vector3 max;
            private float halfSize;

            public BoundingBox()
            {
                min = Vector3.Zero;
                max = Vector3.Zero;
                halfSize = -1.0f;
            }

            public BoundingBox(Vector3 min, Vector3 max)
            {
                this.min = min;
                this.max = max;

                halfSize = (float)Math.Max(Math.Abs(max.X), Math.Max(Math.Abs(max.Y), Math.Abs(max.Z)));
                halfSize = (float)Math.Max(halfSize, Math.Max(Math.Abs(min.X), Math.Max(Math.Abs(min.Y), Math.Abs(min.Z))));
            }

            public void SetBoundingBox(Vector3 min, Vector3 max)
            {
                this.min = min;
                this.max = max;
            }

            public bool IsPointInside(Vector3 p)
            {
                if (p.X >= min.X && p.Y >= min.Y && p.Z >= min.Z &&
                    p.X <= max.X && p.Y <= max.Y && p.Z <= max.Z)
                    return true;
                return false;
            }

            public Vector3 Center
            {
                get
                {
                    return .5f * (min + max);
                }
            }

            public float HalfSize
            {
                get
                {
                    if (halfSize < 0)
                    {
                        halfSize = (float)Math.Max(Math.Abs(max.X), Math.Max(Math.Abs(max.Y), Math.Abs(max.Z)));
                        halfSize = (float)Math.Max(halfSize, Math.Max(Math.Abs(min.X), Math.Max(Math.Abs(min.Y), Math.Abs(min.Z))));
                    }
                    return halfSize;
                }
            }
        };

        public static void GenTriGrid(int numVertRows, int numVertCols, float dx, float dz,
                                Vector3 center, out Vector3[] verts, out int[] indices)
        {
            int numVertices = numVertRows * numVertCols;
            int numCellRows = numVertRows - 1;
            int numCellCols = numVertCols - 1;

            int mNumTris = numCellRows * numCellCols * 2;

            float width = (float)numCellCols * dx;
            float depth = (float)numCellRows * dz;

            //===========================================
            // Build vertices.

            // We first build the grid geometry centered about the origin and on
            // the xz-plane, row-by-row and in a top-down fashion.  We then translate
            // the grid vertices so that they are centered about the specified 
            // parameter 'center'.

            //verts.resize(numVertices);
            verts = new Vector3[numVertices];

            // Offsets to translate grid from quadrant 4 to center of 
            // coordinate system.
            float xOffset = -width * 0.5f;
            float zOffset = depth * 0.5f;

            int k = 0;
            for (float i = 0; i < numVertRows; ++i)
            {
                for (float j = 0; j < numVertCols; ++j)
                {
                    // Negate the depth coordinate to put in quadrant four.  
                    // Then offset to center about coordinate system.
                    verts[k] = new Vector3(0, 0, 0);
                    verts[k].X = j * dx + xOffset;
                    verts[k].Z = -i * dz + zOffset;
                    verts[k].Y = 0.0f;

                    // Translate so that the center of the grid is at the
                    // specified 'center' parameter.
                    //D3DXMATRIX T;
                    //D3DXMatrixTranslation(&T, center.x, center.y, center.z);
                    //D3DXVec3TransformCoord(&verts[k], &verts[k], &T);

                    Matrix translation = Matrix.CreateTranslation(center);
                    verts[ k ] = Vector3.Transform( verts[ k ], translation );
                    //verts[k]. TransformCoordinate(translation);

                    ++k; // Next vertex
                }
            }

            //===========================================
            // Build indices.

            //indices.resize(mNumTris * 3);
            indices = new int[mNumTris * 3];

            // Generate indices for each quad.
            k = 0;
            for (int i = 0; i < numCellRows; ++i)
            {
                for (int j = 0; j < numCellCols; ++j)
                {
                    indices[k] = i * numVertCols + j;
                    indices[k + 1] = i * numVertCols + j + 1;
                    indices[k + 2] = (i + 1) * numVertCols + j;

                    indices[k + 3] = (i + 1) * numVertCols + j;
                    indices[k + 4] = i * numVertCols + j + 1;
                    indices[k + 5] = (i + 1) * numVertCols + j + 1;

                    // next quad
                    k += 6;
                }
            }
        }

        public static void GenTriGridOld(int numVertRows, int numVertCols, float widthOffset, float depthOffset,
                                Vector3 center, out Vector3[] verts, out int[] indices)
        {
            int numVerts = numVertRows * numVertCols;
            int numCellRows = numVertRows - 1;
            int numCellCols = numVertCols - 1;
            int numTris = numCellCols * numCellRows * 2;

            /*     X  <<< width >>>
             *  _ _ _ _ _ _ _ _
             * |_|_|_|_|_|_|_|_|    ^
             * |_|_|_|_|_|_|_|_|    ^
             * |_|_|_|_|_|_|_|_|   depth    Z
             * |_|_|_|_|_|_|_|_|    V
             * |_|_|_|_|_|_|_|_|    V
             * |_|_|_|_|_|_|_|_|
             * 
             */

            float width = (float)numCellCols * widthOffset;
            float depth = (float)numCellRows * depthOffset;

            //===========================================
            // Build vertices.

            // We first build the grid geometry centered about the origin and on
            // the xz-plane, row-by-row and in a top-down fashion.  We then translate
            // the grid vertices so that they are centered about the specified 
            // parameter 'center'.

            verts = new Vector3[numVerts];

            // Offsets to translate grid from quadrant 4 to center of 
            // coordinate system.
            float startX = -width * 0.5f;
            float startZ = depth * 0.5f;

            float endX = width * 0.5f;
            float endZ = -depth * 0.5f;


            Vector3 pos = new Vector3(0.0f, 0.0f, 0.0f);
            int k = 0;
            int i = 0;
            int index = 0;
            for (float z = startZ; z >= endZ; z -= widthOffset)
            {
                int j = 0;
                for (float x = startX; x <= endX; x += widthOffset)
                {
                    // compute the correct index into the vertex buffer and heightmap
                    // based on where we are in the nested loop.
                    index = i * numVertRows + j;

                    // pos.X = x;
                    //pos.Y = 0.0f;
                    //pos.Z = z;
                    verts[index].X = x;
                    verts[index].Y = 0.0f;
                    verts[index].Z = z;
                    j++; // next column
                }
                i++; // next row
            }

            //===========================================
            // Build indices.
            indices = new int[numTris * 3];

            //gernerate indices for each quad
            k = 0;
            for (i = 0; i < numCellRows; i++)
            {
                for (int j = 0; j < numCellCols; j++)
                {
                    indices[k] = i * numVertCols + j;
                    indices[k + 1] = i * numVertCols + j + 1;
                    indices[k + 2] = (i + 1) * numVertCols + j;
                    indices[k + 3] = (i + 1) * numVertCols + j;
                    indices[k + 4] = i * numVertCols + j + 1;
                    indices[k + 5] = (i + 1) * numVertCols + j + 1;

                    //next quad
                    k += 6;
                }
            }
        }

        public static float GetRandomFloat(float a, float b, Random rand)
        {
            if (a >= b) // bad input
                return a;

            // Get random float in [0, 1] interval.
            //Random rand = new Random();
            //float k = rand.Next((int)a, (int)b);
            //return k;
            float f = (float)((rand.Next() % b + a));
            return f;
            //return (f * (b - a)) + a;
        }

        // Returns a random unit vector on the unit sphere.
        public static Vector3 GetRandomVector(float a, float b, Random rand)
        {
            Vector3 vector = new Vector3();
            vector.X = GetRandomFloat(a, b, rand);
            vector.Y = GetRandomFloat(a, b, rand);
            vector.Z = GetRandomFloat(a, b, rand);

            // Project onto unit sphere.
            //vector.Normalize();

            return vector;
        }

        public static float GetRandomFloat(float a, float b)
        {
            if (a >= b) // bad input
                return a;

            // Get random float in [0, 1] interval.
            Random rand = new Random();
            float f = (float)((rand.NextDouble() % 10001) * 0.0001f);

            return (f * (b - a)) + a;
        }

        // Returns a random unit vector on the unit sphere.
        public static void GetRandomVec(out Vector3 vector)
        {
            vector = new Vector3();
            vector.X = GetRandomFloat(-1.0f, 1.0f);
            vector.Y = GetRandomFloat(-1.0f, 1.0f);
            vector.Z = GetRandomFloat(-1.0f, 1.0f);

            // Project onto unit sphere.
            vector.Normalize();
        }

        public static float Saturate(float x)
        {
            if (x < 0.0f)
                x = 0.0f;
            if (x > 1.0f)
                x = 1.0f;

            return x;
        }

        public static float Saturate(float min, float max, float x)
        {
            if (x < min)
                x = min;
            if (x > max)
                x = max;

            return x;
        }

        public static Vector3 Saturate(Vector3 min, Vector3 max, Vector3 color)
        {
            if (color.X < min.X && color.Y < min.Y && color.Z < min.Z)
                color = min;
            if (color.X > max.X && color.Y > max.Y && color.Z > max.Z)
                color = max;

            return color;
        }

        public static float Lerp(float x, float y, float s)
        {
            return x + s * (y - x);
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            return (float)Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2) + Math.Pow(b.Z - a.Z, 2));
        }

        public static Vector3 MultiplyVector3(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X * v2.X,
                               v1.Y * v2.Y,
                               v1.Z * v2.Z);
        }
    }
}
