using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework.Graphics;
using SlimDX;
using XnaVector3 = Microsoft.Xna.Framework.Vector3;
using XnaBoundingBox = Microsoft.Xna.Framework.BoundingBox;


namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// This class is responsible for working with the GeometryData of an IMesh?
    /// TODO: this is getting suspiciously non-responsibility based :)
    /// This is a helper class which allows for easy mesh composition, mainly usefull for debugging visualization
    /// Also includes functions to calculate mesh bounding boxes
    /// </summary>
    public class MeshBuilder
    {
        private List<Microsoft.Xna.Framework.Vector3> positions = new List<Microsoft.Xna.Framework.Vector3>();
        private List<Microsoft.Xna.Framework.Vector3> normals = new List<Microsoft.Xna.Framework.Vector3>();
        private List<Microsoft.Xna.Framework.Vector2> texcoords = new List<Microsoft.Xna.Framework.Vector2>();

        public void AddBox(Vector3 min, Vector3 max)
        {
            TangentVertex[] vertices;
            short[] indices;
            BoxMesh.CreateUnitBoxVerticesAndIndices(out vertices, out indices);

            for (int i = 0; i < indices.Length; i++)
            {

                var pos = vertices[indices[i]].pos;

                pos.X = pos.X * (max.X - min.X);
                pos.Y = pos.Y * (max.Y - min.Y);
                pos.Z = pos.Z * (max.Z - min.Z);

                positions.Add(pos + min.xna());
                normals.Add(vertices[indices[i]].normal);
                texcoords.Add(vertices[indices[i]].uv);

            }
        }

        public void AddSphere(int segments, float radius)
        {
            TangentVertex[] vertices;
            short[] indices;
            // Source: http://local.wasp.uwa.edu.au/~pbourke/miscellaneous/sphere_cylinder/

            // Maak ringen van vertices van onder naar boven


            int i = 0;

            float phi, theta;
            float phiStep, thetaStep;
            float phiStart, phiEnd, thetaStart, thetaEnd;
            phiStep = MathHelper.TwoPi / segments;
            thetaStep = MathHelper.Pi / segments;

            phiStart = 0;
            phiEnd = MathHelper.TwoPi;
            thetaStart = -MathHelper.PiOver2 + thetaStep;
            thetaEnd = MathHelper.PiOver2;

            int numRings = (int)Math.Round((thetaEnd - thetaStart) / thetaStep);
            int numVertsOnRing = (int)Math.Round((phiEnd - phiStart) / phiStep);




            int numVertices = 1 + numRings * numVertsOnRing + 1;


            vertices = new TangentVertex[numVertices];

            // Bottom vertex: (0,-1,0)
            vertices[i].pos = new Microsoft.Xna.Framework.Vector3(0, -1, 0);
            i++;

            theta = thetaStart;
            for (int iRing = 0; iRing < numRings; iRing++, theta += thetaStep)
            {
                phi = 0;
                for (int iVert = 0; iVert < numVertsOnRing; iVert++, phi += phiStep)
                {
                    vertices[i].pos = new Microsoft.Xna.Framework.Vector3(
                        (float)Math.Cos(theta) * (float)Math.Cos(phi),
                        (float)Math.Sin(theta),
                        -(float)Math.Cos(theta) * (float)Math.Sin(phi));
                    i++;
                }
            }
            // Top vertex: (0,1,0)
            vertices[i].pos = new Microsoft.Xna.Framework.Vector3(0, 1, 0);
            i++;


            // Generate normals
            for (int j = 0; j < vertices.Length; j++)
            {
                vertices[j].normal = Microsoft.Xna.Framework.Vector3.Normalize(vertices[j].pos);
            }


            int numIndices = (numVertsOnRing * 2 * 3) * numRings;

            indices = new short[numIndices];
            i = 0;

            // Triangle fan at bottom and top, elsewhere strips between the rings

            // Top and bottom fan

            for (int iVert = 0; iVert < numVertsOnRing - 1; iVert++)
            {
                // Bottom fan
                indices[i] = (short)(0); i++;
                indices[i] = (short)(1 + iVert); i++;
                indices[i] = (short)(1 + (iVert + 1)); i++;

                // Top fan
                indices[i] = (short)(numVertices - 1); i++;
                indices[i] = (short)(1 + (numRings - 1) * numVertsOnRing + (iVert + 1)); i++;
                indices[i] = (short)(1 + (numRings - 1) * numVertsOnRing + iVert); i++;
            }

            // Top and bottom final fan
            indices[i] = (short)(0); i++;
            indices[i] = (short)(1 + numVertsOnRing - 1); i++;
            indices[i] = (short)(1 + 0); i++;

            indices[i] = (short)(numVertices - 1); i++;
            indices[i] = (short)(1 + (numRings - 1) * numVertsOnRing + 0); i++;
            indices[i] = (short)(1 + (numRings - 1) * numVertsOnRing + numVertsOnRing - 1); i++;

            // Strips
            for (int iRing = 0; iRing < numRings - 1; iRing++)
            {
                for (int iVert = 0; iVert < numVertsOnRing - 1; iVert++)
                {
                    indices[i] = (short)(1 + numVertsOnRing * iRing + iVert); i++;
                    indices[i] = (short)(1 + numVertsOnRing * (iRing + 1) + iVert); i++;
                    indices[i] = (short)(1 + numVertsOnRing * iRing + (iVert + 1)); i++;

                    indices[i] = (short)(1 + numVertsOnRing * iRing + (iVert + 1)); i++;
                    indices[i] = (short)(1 + numVertsOnRing * (iRing + 1) + iVert); i++;
                    indices[i] = (short)(1 + numVertsOnRing * (iRing + 1) + (iVert + 1)); i++;
                }
                // Final gap:
                indices[i] = (short)(1 + numVertsOnRing * iRing + (numVertsOnRing - 1)); i++;
                indices[i] = (short)(1 + numVertsOnRing * (iRing + 1) + (numVertsOnRing - 1)); i++;
                indices[i] = (short)(1 + numVertsOnRing * iRing + (0)); i++;

                indices[i] = (short)(1 + numVertsOnRing * iRing + (0)); i++;
                indices[i] = (short)(1 + numVertsOnRing * (iRing + 1) + (numVertsOnRing - 1)); i++;
                indices[i] = (short)(1 + numVertsOnRing * (iRing + 1) + (0)); i++;
            }


            foreach (var index in indices)
            {
                this.positions.Add(vertices[index].pos * radius);
                this.normals.Add(vertices[index].normal);
            }

        }

        public IMesh CreateMesh()
        {
            var geom = new MeshPartGeometryData();
            geom.Sources.Add(new MeshPartGeometryData.Source
            {
                DataVector3 = positions.ToArray(),
                Number = 0,
                Semantic = MeshPartGeometryData.Semantic.Position
            });
            geom.Sources.Add(new MeshPartGeometryData.Source
            {
                DataVector3 = normals.ToArray(),
                Number = 0,
                Semantic = MeshPartGeometryData.Semantic.Normal
            });
            geom.Sources.Add(new MeshPartGeometryData.Source
            {
                DataVector2= texcoords.ToArray(),
                Number = 0,
                Semantic = MeshPartGeometryData.Semantic.Texcoord
            });


            var part = new RAMMeshPart();
            part.SetGeometryData(geom);

            var mesh = new RAMMesh();
            mesh.GetCoreData().Parts.Add(new MeshCoreData.Part
                                             {
                                                 MeshMaterial = new MeshCoreData.Material { DiffuseColor = Color.White },
                                                 MeshPart = part,
                                                 ObjectMatrix = Matrix.Identity.xna()
                                             });
            return mesh;
        }


        /// <summary>
        /// TODO: write tests for boundingbox calculation
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static BoundingBox CalculateBoundingBox(IMesh mesh)
        {
            var ret = new XnaBoundingBox();
            foreach (var part in mesh.GetCoreData().Parts)
            {
                var positions = part.MeshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position);
                if (positions.Length == 0) continue;

                //TODO: check this!
                XnaVector3.Transform(positions, ref part.ObjectMatrix, positions);
                ret.MergeWith(XnaBoundingBox.CreateFromPoints(positions));

            }
            return ret.dx();
        }
        public static BoundingBox CalculateBoundingBox(IMeshPart mesh)
        {
            var positions = mesh.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position);
            return Microsoft.Xna.Framework.BoundingBox.CreateFromPoints(positions).dx();
        }

        /// <summary>
        /// TODO: dont cheat :P
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static BoundingSphere CalculateBoundingSphere(IMesh mesh)
        {
            return Microsoft.Xna.Framework.BoundingSphere.CreateFromBoundingBox(CalculateBoundingBox(mesh).xna()).dx();
        }
        public static BoundingSphere CalculateBoundingSphere(IMeshPart mesh)
        {
            var positions = mesh.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position);
            return Microsoft.Xna.Framework.BoundingSphere.CreateFromPoints(positions).dx();
        }

        /// <summary>
        /// Appends the source mesh to the destination mesh, this is NOT a deep copy!!!
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="worldMatrix"></param>
        public static void AppendMeshTo(IMesh source, IMesh destination, Matrix worldMatrix)
        {
            //TODO: BUG!!! transform the collision data!!!!
            //destination.GetCollisionData().Boxes.AddRange(source.GetCollisionData().Boxes);
            //destination.GetCollisionData().ConvexMeshes.AddRange(source.GetCollisionData().ConvexMeshes);
            //Triangles not supported atm!! destination.GetCollisionData().TriangleMesh. .AddRange(source.GetCollisionData().Boxes);
            foreach (var part in source.GetCoreData().Parts)
            {
                destination.GetCoreData().Parts.Add(new MeshCoreData.Part
                                                        {
                                                            MeshMaterial = part.MeshMaterial,
                                                            MeshPart = part.MeshPart,
                                                            ObjectMatrix = worldMatrix.xna() * part.ObjectMatrix
                                                        });
            }
        }

    }
}
