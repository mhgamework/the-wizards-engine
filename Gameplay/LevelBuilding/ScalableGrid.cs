using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.LevelBuilding
{
    public class ScalableGrid
    {
        public float NodeXSize { get; private set; }
        public float NodeYSize { get; private set; }
        public float NodeZSize { get; private set; }
        private Plane heightPlane;
        private const float minNodeSize = 0.5f;

        public ScalableGrid()
        {
            NodeXSize = 1;
            NodeYSize = 1;
            NodeZSize = 1;
            heightPlane = new Plane(0, 1, 0, 0);
        }

        public void SetNodeXSize(float amount)
        {
            NodeXSize = amount;
            if (NodeXSize < minNodeSize)
                NodeXSize = minNodeSize;
        }

        public void SetNodeYSize(float amount)
        {
            NodeYSize = amount;
            if (NodeYSize < minNodeSize)
                NodeYSize = minNodeSize;
        }

        public void SetNodeZSize(float amount)
        {
            NodeZSize = amount;
            if (NodeZSize < minNodeSize)
                NodeZSize = minNodeSize;
        }

        public void AdjustHeight(bool increase)
        {
            if (increase)
                heightPlane.D += NodeYSize;
            else
                heightPlane.D -= NodeYSize;
        }

        public Vector3 GetSelectedPosition(Ray ray)
        {
            Vector3 pos = new Vector3();
            var intersects = ray.xna().Intersects(heightPlane.xna());

            if (intersects.HasValue)
            {
                pos = ray.Position + intersects.Value * ray.Direction;
            }

            pos.X = pos.X - (pos.X % NodeXSize);
            pos.Y = heightPlane.D;
            pos.Z = pos.Z - (pos.Z % NodeZSize);

            return pos;
        }
    }
}
