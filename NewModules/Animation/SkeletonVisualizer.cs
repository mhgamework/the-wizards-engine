using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Animation
{
    public class SkeletonVisualizer
    {
        public void VisualizeSkeleton(IXNAGame game, Skeleton skeleton)
        {
            VisualizeSkeleton(game, skeleton, Vector3.Zero);
        }
        public void VisualizeSkeleton(IXNAGame game, Skeleton skeleton, Vector3 offset)
        {
            for (int i = 0; i < skeleton.Joints.Count; i++)
            {
                var joint = skeleton.Joints[i];



                var c = Vector3.Transform(Vector3.Zero, joint.AbsoluteMatrix) + offset;
                var x = Vector3.Transform(Vector3.UnitX, joint.AbsoluteMatrix) + offset;
                var y = Vector3.Transform(Vector3.UnitY, joint.AbsoluteMatrix) + offset;
                var z = Vector3.Transform(Vector3.UnitZ, joint.AbsoluteMatrix) + offset;

                var p = c;
                if (joint.Parent != null)
                    p = Vector3.Transform(Vector3.Zero, joint.Parent.AbsoluteMatrix) + offset; 


                game.LineManager3D.AddLine(p, c, Color.White);

                game.LineManager3D.AddLine(c, y, Color.Green);
                game.LineManager3D.AddLine(c, z, Color.Blue);
                game.LineManager3D.AddLine(c, x, Color.Red);
            }
        }
    }
}
