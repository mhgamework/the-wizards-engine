using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Entity
{


    /*
    Derived from
    Lengyel, Eric. “Computing Tangent Space Basis Vectors for an Arbitrary Mesh”. Terathon Software 3D Graphics Library, 2001.
    http://www.terathon.com/code/tangent.html
    */

    /// <summary>
    /// TODO: test
    /// </summary>
    public class TangentSolver
    {
        public Vector4[] GenerateTangents(Vector3[] positions, Vector3[] normals, Vector2[] texcoords)
        {
            int vertexCount = positions.Length;
            Vector3[] vertices = positions;
            //triangles = theMesh.triangles;
            //triangleCount = triangles.length/3;
            int triangleCount = vertexCount / 3;
            Vector4[] tangents = new Vector4[vertexCount];
            Vector3[] tan1 = new Vector3[vertexCount];
            Vector3[] tan2 = new Vector3[vertexCount];
            int tri = 0;
            for (int i = 0; i < (triangleCount); i++)
            {
                /*i1 = triangles[tri];
                i2 = triangles[tri + 1];
                i3 = triangles[tri + 2];*/
                int i1 = i * 3;
                int i2 = i * 3 + 1;
                int i3 = i * 3 + 2;

                Vector3 v1 = vertices[i1];
                Vector3 v2 = vertices[i2];
                Vector3 v3 = vertices[i3];

                Vector2 w1 = texcoords[i1];
                Vector2 w2 = texcoords[i2];
                Vector2 w3 = texcoords[i3];

                float x1 = v2.X - v1.X;
                float x2 = v3.X - v1.X;
                float y1 = v2.Y - v1.Y;
                float y2 = v3.Y - v1.Y;
                float z1 = v2.Z - v1.Z;
                float z2 = v3.Z - v1.Z;

                float s1 = w2.X - w1.X;
                float s2 = w3.X - w1.X;
                float t1 = w2.Y - w1.Y;
                float t2 = w3.Y - w1.Y;

                float r = 1.0f / (s1 * t2 - s2 * t1);
                Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;

                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;

                tri += 3;
            }

            for (int i = 0; i < (vertexCount); i++)
            {
                Vector3 n = normals[i];
                Vector3 t = tan1[i];

                // Gram-Schmidt orthogonalize
                //Vector3.OrthoNormalize(n, t);  //TODO: check next line

                t = Vector3.Normalize((t - n * Vector3.Dot(n, t)));

                tangents[i].X = t.X;
                tangents[i].Y = t.Y;
                tangents[i].Z = t.Z;

                // Calculate handedness
                tangents[i].W = (Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0f) ? -1.0f : 1.0f;
            }
            return tangents;
        }
    }
}