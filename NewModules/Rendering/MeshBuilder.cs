using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework.Graphics;
using SlimDX;

namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// This is a helper class which allows for easy mesh composition, mainly usefull for debugging visualization
    /// TODO: Use texturefactory
    /// </summary>
    public class MeshBuilder
    {
        private List<Microsoft.Xna.Framework.Vector3> positions = new List<Microsoft.Xna.Framework.Vector3>();
        private List<Microsoft.Xna.Framework.Vector3> normals = new List<Microsoft.Xna.Framework.Vector3>();
        private List<Microsoft.Xna.Framework.Vector2> uvs = new List<Microsoft.Xna.Framework.Vector2>();


        private RAMTexture texPlaceholder16;

        public MeshBuilder()
        {
            texPlaceholder16 = new RAMTexture();
            texPlaceholder16.GetCoreData().StorageType = TextureCoreData.TextureStorageType.Disk;
            texPlaceholder16.GetCoreData().DiskFilePath = TWDir.GameData.CreateSubdirectory("Core") +
                                                          "\\Placeholder16.png";

        }

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
                uvs.Add(vertices[indices[i]].uv);


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
                DataVector2 = uvs.ToArray(),
                Number = 0,
                Semantic = MeshPartGeometryData.Semantic.Texcoord
            });

            var part = new RAMMeshPart();
            part.SetGeometryData(geom);

            var mesh = new RAMMesh();
            mesh.GetCoreData().Parts.Add(new MeshCoreData.Part
                                             {
                                                 MeshMaterial = new MeshCoreData.Material { DiffuseColor = Color.White, DiffuseMap = texPlaceholder16 },
                                                 MeshPart = part,
                                                 ObjectMatrix = Matrix.Identity.xna()
                                             });
            return mesh;
        }
    }
}
