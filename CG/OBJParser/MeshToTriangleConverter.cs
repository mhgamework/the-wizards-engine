using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.CG.GeometricSurfaces;
using MHGameWork.TheWizards.CG.Lighting;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Shading;
using MHGameWork.TheWizards.CG.Spatial;
using MHGameWork.TheWizards.CG.Texturing;

namespace MHGameWork.TheWizards.CG.OBJParser
{
    /// <summary>
    /// Responsible for creating triangle objects from IMesh objects
    /// </summary>
    public class MeshToTriangleConverter
    {

        private SimpleTexture2DLoader loader = new SimpleTexture2DLoader();
        public List<TriangleGeometry> GetTrianglesWithPhong(RAMMesh mesh, Func<PhongShader> phongFactory)
        {
            var ret = new List<TriangleGeometry>();
            foreach (var part in mesh.GetParts())
            {
                var shader = phongFactory();

                if (part.MeshMaterial.DiffuseMap != null)
                {
                    shader.Diffuse = new Texture2DGeometrySampler(new Texture2DSampler(), loader.Load(part.MeshMaterial.DiffuseMap));
                }
                else
                    continue;

                var positions = part.Positions;
                var normals = part.Normals;
                var texcoords = part.Texcoords;

                var vertices = positions.Select((p, i) => new TangentVertex(positions[i], texcoords[i], normals[i], new Vector3())).ToArray();

                //Microsoft.Xna.Framework.Vector3.Transform(positions, ref part.ObjectMatrix, positions);
                for (int i = 0; i < positions.Length / 3; i++)
                {
                    ret.Add(new TriangleGeometry(vertices, i));
                }
            }
            return ret;
        }

        public List<TriangleGeometry> GetTriangles(RAMMesh mesh, IShader shader)
        {
            var ret = new List<TriangleGeometry>();
            foreach (var part in mesh.GetParts())
            {
                var positions = part.Positions; 
                var normals = part.Normals;
                var texcoords = part.Texcoords;

                var vertices = positions.Select((p, i) => new TangentVertex(positions[i], texcoords[i], normals[i], new Vector3())).ToArray();

                //Microsoft.Xna.Framework.Vector3.Transform(positions, ref part.ObjectMatrix, positions);
                for (int i = 0; i < positions.Length/3; i ++)
                {
                    ret.Add(new TriangleGeometry(vertices, i));
                }
            }
            return ret;
        }

        public List<Triangle> GetTriangles(RAMMesh mesh)
        {
            var ret = new List<Triangle>();
            foreach (var part in mesh.GetParts())
            {
                var positions = part.Positions;
                //Microsoft.Xna.Framework.Vector3.Transform(positions, ref part.ObjectMatrix, positions);
                for (int i = 0; i < positions.Length; i+=3)
                {
                    ret.Add(new Triangle(positions[i], positions[i + 1], positions[i + 2]));
                }
            }
            return ret;
        }
    }
}
