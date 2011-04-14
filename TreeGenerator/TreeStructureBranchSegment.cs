using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TreeGenerator
{
    public class TreeStructureBranchSegment
    {
        public float Length;
        public Vector3 Direction;
        /// <summary>
        /// De straal op het einde van deze segment.
        /// </summary>
        public float EndRadius;
    }
}
