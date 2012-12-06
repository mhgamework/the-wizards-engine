using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing.Surfaces;
using MHGameWork.TheWizards.CG.Shading;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.CG.Spatial
{
    /// <summary>
    /// Responsible for creating triangle objects from IMesh objects
    /// </summary>
    public class MeshToTriangleConverter
    {
        public List<TriangleSurface> GetTriangles(IMesh mesh, IShader shader)
        {
            var ret = new List<TriangleSurface>();
            foreach (var part in mesh.GetCoreData().Parts)
            {
                var positions = part.MeshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position);
                var normals = part.MeshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Normal);
                var texcoords = part.MeshPart.GetGeometryData().GetSourceVector2(MeshPartGeometryData.Semantic.Texcoord);

                var vertices = positions.Select((p, i) => new TangentVertex(positions[i].dx().cg(), texcoords[i].dx().cg(), normals[i].dx().cg(), new Vector3())).ToArray();

                Microsoft.Xna.Framework.Vector3.Transform(positions, ref part.ObjectMatrix, positions);
                for (int i = 0; i < positions.Length/3; i ++)
                {
                    ret.Add(new TriangleSurface(vertices, i, shader));
                }
            }
            return ret;
        }

        public List<Triangle> GetTriangles(IMesh mesh)
        {
            var ret = new List<Triangle>();
            foreach (var part in mesh.GetCoreData().Parts)
            {
                var positions = part.MeshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position);
                Microsoft.Xna.Framework.Vector3.Transform(positions, ref part.ObjectMatrix, positions);
                for (int i = 0; i < positions.Length; i+=3)
                {
                    ret.Add(new Triangle(positions[i].dx().cg(), positions[i + 1].dx().cg(), positions[i + 2].dx().cg()));
                }
            }
            return ret;
        }
    }
}
