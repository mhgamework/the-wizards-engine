using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.CG.Spatial
{
    /// <summary>
    /// Responsible for creating triangle objects from IMesh objects
    /// </summary>
    public class MeshToTriangleConverter
    {
        public List<Triangle> GetTriangles(IMesh mesh)
        {
            var ret = new List<Triangle>();
            foreach (var part in mesh.GetCoreData().Parts)
            {
                var positions = part.MeshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position);
                Microsoft.Xna.Framework.Vector3.Transform(positions, ref part.ObjectMatrix, positions);
                for (int i = 0; i < positions.Length; i+=3)
                {
                    ret.Add(new Triangle(positions[i].dx(), positions[i + 1].dx(), positions[i + 2].dx()));
                }
            }
            return ret;
        }
    }
}
