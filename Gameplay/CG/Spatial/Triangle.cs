using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.CG.Spatial
{
    public class Triangle : ISurface
    {
        private Vector3[] positions = new Vector3[3];

        public Triangle(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            positions[0] = v0;
            positions[1] = v1;
            positions[2] = v2;
        }

        public Vector3 getPosition(int vertexIndex)
        {
            return positions[vertexIndex];
        }

        public BoundingBox GetBoundingBox(IBoundingBoxCalculator calc)
        {
            return calc.GetBoundingBox(this);
        }

        public Vector3[] getPositions()
        {
            return positions;
        }
    }
}
