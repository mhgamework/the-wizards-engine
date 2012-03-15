using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.Model
{
    public class Sphere : BaseModelObject
    {

        public Sphere()
        {
            var ent = new Entity();

            ent.Mesh = createSphereMesh();

        }

        private static RAMMesh createSphereMesh()
        {
            MeshPartGeometryData.Source source;


            TangentVertex[] vertices;
            short[] indices;
            SphereMesh.CreateUnitSphereVerticesAndIndices(12, out vertices, out indices);


            var mesh = new RAMMesh();
            mesh.GetCoreData().Parts.Add(new MeshCoreData.Part());
            var part = new RAMMeshPart();
            mesh.GetCoreData().Parts[0].MeshPart = part;
            mesh.GetCoreData().Parts[0].ObjectMatrix = Matrix.Identity.xna();


            part.GetGeometryData().SetSourcesFromTangentVertices(indices, vertices);
            return mesh;
        }
    }
}
