﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.OBJParser;
using MHGameWork.TheWizards.CG.Raytracing.Surfaces;
using MHGameWork.TheWizards.CG.Shading;

namespace MHGameWork.TheWizards.CG.Spatial
{
    /// <summary>
    /// Responsible for creating triangle objects from IMesh objects
    /// </summary>
    public class MeshToTriangleConverter
    {
        public List<TriangleSurface> GetTriangles(RAMMesh mesh, IShader shader)
        {
            var ret = new List<TriangleSurface>();
            foreach (var part in mesh.GetParts())
            {
                var positions = part.Positions; 
                var normals = part.Normals;
                var texcoords = part.Texcoords;

                var vertices = positions.Select((p, i) => new TangentVertex(positions[i], texcoords[i], normals[i], new Vector3())).ToArray();

                //Microsoft.Xna.Framework.Vector3.Transform(positions, ref part.ObjectMatrix, positions);
                for (int i = 0; i < positions.Length/3; i ++)
                {
                    ret.Add(new TriangleSurface(vertices, i, shader));
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
