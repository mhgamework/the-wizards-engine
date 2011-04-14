using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Physics
{
    public class MeshCollisionData
    {
        public List<Box> Boxes = new List<Box>();

        public List<Convex> ConvexMeshes = new List<Convex>();

        // Currently one triangle mesh allowed!
        public TriangleMeshData TriangleMesh;


        public class TriangleMeshData
        {
            public List<Vector3> Positions = new List<Vector3>();
            public List<int> Indices = new List<int>();

        }
        public class Box
        {
            public Vector3 Dimensions;
            public Matrix Orientation;
        }
        public class Convex
        {
            public List<Vector3> Positions = new List<Vector3>();
        }
    }
}
