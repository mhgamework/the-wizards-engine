using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.WorldRendering;
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

        private Textarea textarea;

        public ScalableGrid()
        {
            NodeXSize = 1;
            NodeYSize = 1;
            NodeZSize = 1;
            heightPlane = new Plane(0, 1, 0, 0);

            textarea = new Textarea();
            textarea.Color = new Color4(1, 1, 1);
            textarea.Position = new Vector2(10, 10);
            textarea.Visible = true;
            textarea.Size = new Vector2(500, 500);
        }

        public void UpdateDraw(Vector3 center)
        {
            int size = 5; //# lines to draw * 0.5
            var gridColor = new Color4(1, 1, 1);

            TW.Graphics.LineManager3D.DrawGroundShadows = true;

            for (int i = -size; i < size; i++)
            {
                Vector3 start = new Vector3(center.X + NodeXSize * 0.5f + i * NodeXSize, heightPlane.D, center.Z - size * NodeZSize);
                Vector3 end = new Vector3(center.X + NodeXSize * 0.5f + i * NodeXSize, heightPlane.D, center.Z + size * NodeZSize);
                TW.Graphics.LineManager3D.AddLine(start, end, gridColor);

                start = new Vector3(center.X - size * NodeXSize, heightPlane.D, center.Z + NodeZSize * 0.5f + i * NodeZSize);
                end = new Vector3(center.X + size * NodeXSize, heightPlane.D, center.Z + NodeZSize * 0.5f + i * NodeZSize);
                TW.Graphics.LineManager3D.AddLine(start, end, gridColor);
            }

            var selectionColor = new Color4(1, 0, 0);
            Vector3 selectionStart = new Vector3(center.X - NodeXSize * 0.5f, heightPlane.D, center.Z - NodeZSize * 0.5f);

            TW.Graphics.LineManager3D.AddLine(selectionStart, selectionStart + new Vector3(NodeXSize, 0, 0),
                                          selectionColor);
            TW.Graphics.LineManager3D.AddLine(selectionStart + new Vector3(NodeXSize, 0, 0), selectionStart + new Vector3(NodeXSize, 0, NodeZSize),
                                          selectionColor);
            TW.Graphics.LineManager3D.AddLine(selectionStart + new Vector3(NodeXSize, 0, NodeZSize), selectionStart + new Vector3(0, 0, NodeZSize),
                                          selectionColor);
            TW.Graphics.LineManager3D.AddLine(selectionStart + new Vector3(0, 0, NodeZSize), selectionStart,
                                          selectionColor);

            textarea.Text = "NodeXSize: " + NodeXSize + "\nNodeYSize: " + NodeYSize + "\nNodeZSize: " + NodeZSize +
                        "\nHeigth: " + heightPlane.D;
            textarea.Text += "\nPosition: (" + center.X + "| " + heightPlane.D + "| " + center.Z + ")";
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
