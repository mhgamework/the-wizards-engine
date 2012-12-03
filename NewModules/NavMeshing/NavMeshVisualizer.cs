using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using MHGameWork.TheWizards.DirectX11.Graphics;
using SlimDX;

namespace MHGameWork.TheWizards.NavMeshing
{
    public class NavMeshVisualizer
    {
        public LineManager3DLines Lines;

        public NavMeshVisualizer(LineManager3DLines lines)
        {
            Lines = lines;
        }

        public void UpdateLines(NavMesh mesh)
        {
            Lines.ClearAllLines();
            foreach (var node in mesh.Nodes)
            {
                Lines.AddPolygon(getPoints(node), new SlimDX.Color4(Color.White));
            }
        }
        private Vector3[] getPoints(NavMeshNode node)
        {
            return node.Edges.Select(e => e.Point).ToArray();
        }
    }
}
