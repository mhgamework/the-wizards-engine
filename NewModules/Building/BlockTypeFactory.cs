using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Building;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Building
{
    /// <summary>
    /// Responsible for creating new BlockTypes
    /// </summary>
    public class BlockTypeFactory
    {
        public List<BlockType> TypeList = new List<BlockType>();
        private List<Matrix> rotationList = new List<Matrix>();

        public BlockTypeFactory()
        {
            calculateRotations();
        }

        public void AddRotatedBlocktypes(IMesh mesh, BlockLayout layout)
        {
            for (int i = 0; i < rotationList.Count; i++)
            {
                var currentRotation = rotationList[i];
                for (int j = 0; j < BlockLayout.Axes.Count; j++)
                {
                    var k = 0;
                    var axis = BlockLayout.Axes[j];
                    var transformedAxis = Vector3.TransformNormal(axis, currentRotation);
                    var transformedPoint = new Point3((int)Math.Round(transformedAxis.X), (int)Math.Round(transformedAxis.Y), (int)Math.Round(transformedAxis.Z));
                    k |= BlockLayout.GetMask(axis) & BlockLayout.GetMask(transformedPoint);


                    var ret = CreateNewBlockType(mesh, new BlockLayout(k));

                    ret.Transformation = currentRotation;
                }
            }
        }

        public BlockType CreateNewBlockType(IMesh mesh, BlockLayout layout)
        {
            var ret = new BlockType(mesh);
            ret.Layout = layout;
            ret.Transformation = Matrix.RotationX(MathHelper.PiOver2);
            TypeList.Add(ret);

            return ret;
        }

        private void calculateRotations()
        {
            var angles = new float[4];
            for (int i = 0; i < 4; i++)
                angles[i] = MathHelper.PiOver2 * i;

            var storage = angles.SelectMany(angle => new[] { Matrix.RotationX(angle), Matrix.RotationY(angle), Matrix.RotationZ(angle) });
            rotationList.AddRange(storage.Join(storage, o => 1, o => 1, (a, b) => a * b));
        }
    }
}
