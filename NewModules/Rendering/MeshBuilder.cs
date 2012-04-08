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
    /// </summary>
    public class MeshBuilder
    {
        private List<Microsoft.Xna.Framework.Vector3> positions = new List<Microsoft.Xna.Framework.Vector3>();
        private List<Microsoft.Xna.Framework.Vector3> normals = new List<Microsoft.Xna.Framework.Vector3>();

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
    }
}
