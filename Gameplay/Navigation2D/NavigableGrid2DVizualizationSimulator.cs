using System;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.Navigation2D
{
    public class NavigableGrid2DVizualizationSimulator : ISimulator
    {

        private NavigableGrid2DData data = TW.Data.GetSingleton<NavigableGrid2DData>();

        public void Simulate()
        {
            for (int i = 0; i < data.Grid.Width + 1; i++)
                TW.Graphics.LineManager3D.AddLine(new Vector3(i * data.Grid.NodeSize, 0, 0), new Vector3(i * data.Grid.NodeSize, 0, data.Grid.Height * data.Grid.NodeSize), new Color4(1, 1, 1));
            for (int i = 0; i < data.Grid.Height + 1; i++)
                TW.Graphics.LineManager3D.AddLine(new Vector3(0, 0, i * data.Grid.NodeSize), new Vector3(data.Grid.Width * data.Grid.NodeSize, 0, i * data.Grid.NodeSize), new Color4(1, 1, 1));

            for (int x = 0; x < data.Grid.Width; x++)
                for (int y = 0; y < data.Grid.Height; y++)
                {
                    if (data.Grid.IsFree(x, y)) continue;
                    float ix = x * data.Grid.NodeSize;
                    float iy = y * data.Grid.NodeSize;
                    float n = data.Grid.NodeSize;
                    TW.Graphics.LineManager3D.AddLine(new Vector3(ix, 0, iy), new Vector3(ix + n, 0, iy + n), new Color4(1, 0.5f, 0));
                    TW.Graphics.LineManager3D.AddLine(new Vector3(ix + n, 0, iy), new Vector3(ix, 0, iy + n), new Color4(1, 0.5f, 0));

                }
        }
    }
}