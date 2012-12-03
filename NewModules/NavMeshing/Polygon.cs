using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.NavMeshing
{
    public struct Polygon
    {
        public Vector3[] Points;


        public int NumPoints { get { return Points.Length; } }

        /// <summary>
        /// The new point is inserted in the edge vor point 'edgeNum' to 'edgeNum + 1'
        /// Each polygon has as startnode 'point' and the vertices keep their order
        /// The original node holds the first edge vertex, the other node the second
        /// </summary>
        /// <param name="node"></param>
        /// <param name="edgeNum"></param>
        /// <param name="point"></param>
        public void Split(int edgeNum, Vector3 point, out Polygon newPolygon)
        {
            int startI = edgeNum;
            int pivotI = edgeNum - 1;

            var polygon1 = new Vector3[startI - pivotI + 2];
            var polygon2 = new Vector3[NumPoints - polygon1.Length + 3];

            polygon1[0] = point;
            polygon2[0] = point;

            int baseI = pivotI;
            for (int i = 0; i < polygon1.Length - 1; i++)
            {
                int index = (baseI + i + NumPoints) % NumPoints;
                polygon1[i + 1] = Points[index];
            }

            baseI = startI + 1;
            for (int i = 0; i < polygon1.Length - 1; i++)
            {
                int index = (baseI + i + NumPoints) % NumPoints;
                polygon2[i + 1] = Points[index];
            }


            newPolygon = new Polygon();
            newPolygon.Points = polygon2;

            Points = polygon1;

        }
    }
}
