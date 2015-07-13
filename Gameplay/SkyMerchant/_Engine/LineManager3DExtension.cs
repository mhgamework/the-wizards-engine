using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._Engine
{
    public static class LineManager3DExtension
    {
        public static void AddMatrixAxes(this LineManager3D lm, Matrix matrix)
        {
            var oriMat = lm.WorldMatrix;
            lm.WorldMatrix = matrix;

            var axes = new[] {Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ};
            var colors = new[] { new Color4(1, 0, 0, 1), new Color4(0, 1, 0, 1), new Color4(0, 0, 1, 1) };

            for (int i = 0; i < axes.Length; i++)
            {
                lm.AddLine(Vector3.Zero, axes[i], colors[i]);
            }

            lm.WorldMatrix = oriMat;
        }
    }
}
