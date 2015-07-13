using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
            throw new NotImplementedException();
        }

        public static BoundingBox FromPoints(SlimDX.Vector3[] toArray)
        {
            throw new NotImplementedException();
        }

        public static BoundingBox Merge(BoundingBox box1, BoundingBox box2)
        {
            throw new NotImplementedException();
        }
    }
}
