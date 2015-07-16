using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;

namespace MHGameWork.TheWizards
{
    [DebuggerDisplay("Box({Min},{Max}")]
    public struct BoundingBox
    {
        public Vector3 Minimum { get; set; }
        public Vector3 Maximum { get; set; }

        public BoundingBox(Vector3 min, Vector3 max)
            : this()
        {
            Minimum = min;
            Maximum = max;
        }

        public Vector3[] GetCorners()
        {
            return dx().GetCorners().Cast<Vector3>().ToArray();
        }

        public static BoundingBox FromPoints(Vector3[] toArray)
        {
            return SlimDX.BoundingBox.FromPoints(toArray.Select(v => v.dx()).ToArray());
        }

        public static BoundingBox Merge(BoundingBox box1, BoundingBox box2)
        {
            return SlimDX.BoundingBox.Merge(box1, box2);
        }

        public static implicit operator Microsoft.Xna.Framework.BoundingBox(BoundingBox m)
        {
            return new Microsoft.Xna.Framework.BoundingBox(m.Minimum.xna(), m.Maximum.xna());
        }
        public static implicit operator BoundingBox(Microsoft.Xna.Framework.BoundingBox v)
        {
            return new BoundingBox(v.Min, v.Max);

        }
        public static implicit operator SlimDX.BoundingBox(BoundingBox m)
        {
            return new SlimDX.BoundingBox(m.Minimum, m.Maximum);

        }
        public static implicit operator BoundingBox(SlimDX.BoundingBox m)
        {
            return new BoundingBox(m.Minimum, m.Maximum);

        }

        public Microsoft.Xna.Framework.BoundingBox xna() { return this; }
        public SlimDX.BoundingBox dx() { return this; }

        public ContainmentType Contains(Vector3 vector3)
        {
            var c = xna().Contains(vector3.dx().xna());
            switch (c)
            {
                case Microsoft.Xna.Framework.ContainmentType.Contains:
                    return ContainmentType.Contains;
                case Microsoft.Xna.Framework.ContainmentType.Disjoint:
                    return ContainmentType.Disjoint;
                case Microsoft.Xna.Framework.ContainmentType.Intersects:
                    return ContainmentType.Intersects;
                default:
                    throw new InvalidOperationException();

            }
        }
    }
}
