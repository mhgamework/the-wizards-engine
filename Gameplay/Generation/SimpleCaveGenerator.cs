using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using SlimDX;
using ContainmentType = Microsoft.Xna.Framework.ContainmentType;

namespace MHGameWork.TheWizards.Generation
{
    public class SimpleCaveGenerator
    {
        public Random Rand { get; set; }

        public SimpleCaveGenerator()
        {
            Rand = new Random(578);
        }

        public void GenerateRandom(VoxelGrid<bool> grid, float percentEmpty)
        {

            grid.ForEach(delegate(int x, int y, int z)
                {
                    grid[x, y, z] = Rand.NextDouble() > percentEmpty;

                });
        }

        public void ProcessCellularAutomata(VoxelGrid<bool> grid)
        {
            grid.ForEach(delegate(int x, int y, int z)
                {
                    int sum = 0;
                    for (int i = 0; i < 3 * 3 * 3; i++)
                    {
                        int ix = x + i % 3;
                        int iy = y + (i / 3) % 3;
                        int iz = z + (i / 9) % 3;
                        if (!grid.ContainsKey(ix, iy, iz)) ; //sum++;
                        else if (grid[ix, iy, iz]) sum++;
                    }

                    grid[x, y, z] = sum >= 17;

                });
        }

        public void CreateRoom(VoxelGrid<bool> grid, BoundingBox room)
        {
            grid.ForEach(delegate(int x, int y, int z)
            {
                grid[x, y, z] = false;
            }, room);
        }

        public void GenerateRooms(VoxelGrid<bool> grid)
        {
            grid.ForEach(delegate(int x, int y, int z)
                {
                    var size = new Vector3(10, 5, 10);

                    var averageVolume = size.X*size.Y*size.Z;
                    var roomDensity = 4f;
                    if (Rand.NextDouble() > 1/averageVolume/roomDensity) return;
                    var room = new BoundingBox();
                    room.Minimum = new Vector3(x, y, z);
                    
                    room.Maximum = room.Minimum + size;
                    CreateRoom(grid, room);

                });
        }

        public void FillBorders(VoxelGrid<bool> grid)
        {
            grid.ForEach(delegate(int x, int y, int z)
                {
                    var val = false;
                    if (x == 0) val = true;
                    if (y == 0) val = true;
                    if (z == 0) val = true;
                    if (x == grid.Size.X) val = true;
                    if (y == grid.Size.Y) val = true;
                    if (z == grid.Size.Z) val = true;

                    if (val)
                        grid[x, y, z] = true;

                });
        }
    }
}
