using System.Collections.Generic;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    public class HermiteDataGrid
    {
        private List<Vector3> cube_verts;

        private Array3D<Vertex> cells;

        public HermiteDataGrid()
        {
            cube_verts = (from x in Enumerable.Range(0, 2)
                          from y in Enumerable.Range(0, 2)
                          from z in Enumerable.Range(0, 2)
                          select new Vector3(x, y, z)).ToList();


            var cube_edges = (from v in cube_verts
                              from offset in new[] { Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ }
                              where (v + offset).X < 1.5
                              where (v + offset).Y < 1.5
                              where (v + offset).Z < 1.5
                              select new { Start = v, End = v + offset }).Distinct().ToList();

            var edgeToVertices = (from edge in cube_edges
                                  select new
                                  {
                                      Start = cube_verts.IndexOf(edge.Start),
                                      End = cube_verts.IndexOf(edge.End)
                                  }).ToList();
        }

        public bool GetSign(Point3 pos)
        {
            var v = cells[pos];
            if (cells == null) return false;
            return v.Sign;
        }

        public bool[] GetSigns(Point3 cube)
        {
            
        }

        private struct Vertex
        {
            public bool Sign;
            public Vector4[] EdgeData;
        }
        
    }
}