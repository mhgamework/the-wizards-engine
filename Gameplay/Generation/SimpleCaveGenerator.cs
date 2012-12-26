using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Generation
{
    public class SimpleCaveGenerator
    {
        public void GenerateRandom(VoxelGrid<bool> grid)
        {
            var rand = new Random(578);

            grid.ForEach(delegate(int x, int y, int z)
                {
                    grid[x, y, z] = rand.NextDouble() > 0.4f;

                });

        }
    }
}
