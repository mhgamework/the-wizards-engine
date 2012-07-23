using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Animation
{
    public class Skeleton
    {
        /// <summary>
        /// Joints are ordered by tree-depth, rootnode first.
        /// Joints[i].depth is always smaller than Joints[i+1].depth
        /// </summary>
        public List<Joint> Joints = new List<Joint>();

        public void UpdateAbsoluteMatrices()
        {
            for (int i = 0; i < Joints.Count; i++)
            {
                var j = Joints[i];

                j.CalculateAbsoluteMatrix();
            }
        }

    
    }
}
