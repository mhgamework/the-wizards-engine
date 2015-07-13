using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.Animation
{
    public class SkeletonVisualizer
    {
        public void VisualizeSkeleton(DX11Game game, Skeleton skeleton)
        {
            VisualizeSkeleton(game, skeleton, Vector3.Zero);
        }
        public void VisualizeSkeleton(DX11Game game, Skeleton skeleton, Vector3 offset)
        {
            for (int i = 0; i < skeleton.Joints.Count; i++)
            {
                var joint = skeleton.Joints[i];



                var c = Vector3.TransformCoordinate(Vector3.Zero, joint.AbsoluteMatrix) + offset;
                var x = Vector3.TransformCoordinate(Vector3.UnitX, joint.AbsoluteMatrix) + offset;
                var y = Vector3.TransformCoordinate(Vector3.UnitY, joint.AbsoluteMatrix) + offset;
                var z = Vector3.TransformCoordinate(Vector3.UnitZ, joint.AbsoluteMatrix) + offset;

                var p = c;
                if (joint.Parent != null)
                    p = Vector3.TransformCoordinate(Vector3.Zero, joint.Parent.AbsoluteMatrix) + offset;


                game.LineManager3D.AddLine(p, c, new Color4(Color.White));

                game.LineManager3D.AddLine(c, y, new Color4(Color.Green));
                game.LineManager3D.AddLine(c, z, new Color4(Color.Blue));
                game.LineManager3D.AddLine(c, x, new Color4(Color.Red));
            }
        }
    }
}
