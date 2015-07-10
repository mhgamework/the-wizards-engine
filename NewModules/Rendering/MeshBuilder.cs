using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;

using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Graphics.SlimDX.Rendering.TextureMapping;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
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
        private Dictionary<MeshMaterial, MeshData> parts;
        private MeshMaterial defaultMaterial;

        public MeshBuilder()
        {
            parts = new Dictionary<MeshMaterial, MeshData>();
            defaultMaterial = CreateMaterial();
            defaultMaterial.DiffuseColor = Color.White;
        }

        public MeshMaterial CreateMaterial()
        {
            var ret = new MeshMaterial();
            parts[ret] = new MeshData();
            return ret;
        }
        public void AddBox(Vector3 min, Vector3 max)
        {
            AddBox(min, max, defaultMaterial);
        }
        public void AddBox(Vector3 min, Vector3 max, MeshMaterial material)
        {
            if (!parts.ContainsKey(material)) throw new InvalidOperationException("Material not created by the meshbuilder");
            TangentVertex[] vertices;
            short[] indices;
            BoxMesh.CreateUnitBoxVerticesAndIndices(out vertices, out indices);

            var data = parts[material];

            for (int i = 0; i < indices.Length; i++)
            {

                var pos = vertices[indices[i]].pos;

                pos.X = pos.X * (max.X - min.X);
                pos.Y = pos.Y * (max.Y - min.Y);
                pos.Z = pos.Z * (max.Z - min.Z);

                data.Positions.Add(pos + min.xna());
                data.Normals.Add(vertices[indices[i]].normal);
                data.Texcoords.Add(vertices[indices[i]].uv);

            }
        }

        public void AddCustom(Vector3[] nPositions, Vector3[] nNormals, Vector2[] nTexcoords)
        {
            AddCustom(nPositions, nNormals, nTexcoords, defaultMaterial);
        }
        public void AddCustom(Vector3[] nPositions, Vector3[] nNormals, Vector2[] nTexcoords, MeshMaterial material)
        {
            if (!parts.ContainsKey(material)) throw new InvalidOperationException("Material not created by the meshbuilder");
            if (nPositions.Length != nNormals.Length) throw new ArgumentException();
            if (nPositions.Length != nTexcoords.Length) throw new ArgumentException();
            var data = parts[material];
            data.Positions.AddRange(nPositions.Select(v => v.xna()));
            data.Normals.AddRange(nNormals.Select(v => v.xna()));
            data.Texcoords.AddRange(nTexcoords.Select(t => new Microsoft.Xna.Framework.Vector2(t.X, t.Y)));
        }

        public void AddSphere(int segments, float radius)
        {
            AddSphere(segments, radius, defaultMaterial);
        }
        public void AddSphere(int segments, float radius, MeshMaterial material)
        {
            if (!parts.ContainsKey(material)) throw new InvalidOperationException("Material not created by the meshbuilder");
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

            var mapping = new SphericalMapping();

            var data = parts[material];

            foreach (var index in indices)
            {
                data.Positions.Add(vertices[index].pos * radius);
                data.Normals.Add(vertices[index].normal);
                data.Texcoords.Add(mapping.Map(data.Positions[data.Positions.Count - 1].ToSlimDX()).xna() * 3);
            }

        }

        public void AddGeometryData(MeshPartGeometryData data, MeshMaterial material, Matrix transform)
        {
            if (!parts.ContainsKey(material)) throw new InvalidOperationException("Material not created by the meshbuilder");
            var p = parts[material];

            var xTransform = transform.xna();

            var sPositions = data.GetSourceVector3(MeshPartGeometryData.Semantic.Position);
            var sNormals = data.GetSourceVector3(MeshPartGeometryData.Semantic.Normal);
            var sTexcoords = data.GetSourceVector2(MeshPartGeometryData.Semantic.Texcoord);

            var nPositions = new XnaVector3[sPositions.Length];
            var nNormals = new XnaVector3[sNormals.Length];
            var nTexcoords = sTexcoords;

            XnaVector3.Transform(sPositions, ref xTransform, nPositions);
            XnaVector3.TransformNormal(sNormals, ref xTransform, nNormals);

            p.Positions.AddRange(nPositions);
            p.Normals.AddRange(nNormals);
            p.Texcoords.AddRange(nTexcoords);
        }

        public void AddMesh(IMesh mesh, Matrix transform)
        {
            foreach (var p in mesh.GetCoreData().Parts)
            {
                var mat = CreateMaterial();
                mat.SetFromMeshCoreDataMaterial(p.MeshMaterial);

                AddGeometryData(p.MeshPart.GetGeometryData(), mat, p.ObjectMatrix.dx() * transform);
            }
        }

        public IMesh CreateMesh()
        {

            var mesh = new RAMMesh();

            foreach (var pair in parts)
            {
                var mat = pair.Key;
                var data = pair.Value;


                if (data.Positions.Count == 0) continue;

                var geom = new MeshPartGeometryData();
                geom.Sources.Add(new MeshPartGeometryData.Source
                {
                    DataVector3 = data.Positions.ToArray(),
                    Number = 0,
                    Semantic = MeshPartGeometryData.Semantic.Position
                });
                geom.Sources.Add(new MeshPartGeometryData.Source
                {
                    DataVector3 = data.Normals.ToArray(),
                    Number = 0,
                    Semantic = MeshPartGeometryData.Semantic.Normal
                });
                geom.Sources.Add(new MeshPartGeometryData.Source
                {
                    DataVector2 = data.Texcoords.ToArray(),
                    Number = 0,
                    Semantic = MeshPartGeometryData.Semantic.Texcoord
                });
                geom.Sources.Add(new MeshPartGeometryData.Source
                {
                    DataVector3 = calculateTangents(data),
                    Number = 0,
                    Semantic = MeshPartGeometryData.Semantic.Tangent
                });

                var part = new RAMMeshPart();
                part.SetGeometryData(geom);

                mesh.GetCoreData().Parts.Add(new MeshCoreData.Part
                                        {
                                            MeshMaterial = mat.ToMeshCoreDataMaterial(),
                                            MeshPart = part,
                                            ObjectMatrix = Matrix.Identity.xna()
                                        });
            }



            return mesh;
        }

        private XnaVector3[] calculateTangents( MeshData data )
        {
            var solver = new TangentSolver();
            return solver.GenerateTangents(data.Positions.ToArray(), data.Normals.ToArray(), data.Texcoords.ToArray()).Select(v => new Vector3(v.X, v.Y, v.Z).xna()).ToArray();
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
                ret = ret.MergeWith(XnaBoundingBox.CreateFromPoints(positions));

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
                                                            ObjectMatrix = part.ObjectMatrix * worldMatrix.xna()
                                                        });
            }
        }

        public static IMesh DeepCopy(IMesh mesh)
        {
            throw new NotImplementedException();
        }

        public static IMesh Transform(IMesh mesh, Matrix transform)
        {
            //TODO: first take deep copy?
            var ret = new RAMMesh();
            ret.GetCoreData().Parts = mesh.GetCoreData().Parts.Select(p =>
                {
                    var nPart = new MeshCoreData.Part();
                    nPart.ObjectMatrix = p.ObjectMatrix * transform.xna();
                    nPart.MeshMaterial = p.MeshMaterial;
                    nPart.MeshPart = p.MeshPart;
                    return nPart;
                }).ToList();
            return ret;
        }


        public class MeshData
        {
            public List<Microsoft.Xna.Framework.Vector3> Positions = new List<Microsoft.Xna.Framework.Vector3>();
            public List<Microsoft.Xna.Framework.Vector3> Normals = new List<Microsoft.Xna.Framework.Vector3>();
            public List<Microsoft.Xna.Framework.Vector2> Texcoords = new List<Microsoft.Xna.Framework.Vector2>();
        }
        public class MeshMaterial
        {
            public Color DiffuseColor;
            public ITexture DiffuseMap;
            public bool ColoredMaterial = false;

            public MeshCoreData.Material ToMeshCoreDataMaterial()
            {
                return new MeshCoreData.Material()
                    {
                        ColoredMaterial = ColoredMaterial,
                        DiffuseColor = DiffuseColor,
                        DiffuseMap = DiffuseMap
                    };
            }

            public void SetFromMeshCoreDataMaterial(MeshCoreData.Material mat)
            {

                DiffuseColor = mat.DiffuseColor;
                DiffuseMap = mat.DiffuseMap;
                ColoredMaterial = mat.ColoredMaterial;

            }
        }
    }
}
