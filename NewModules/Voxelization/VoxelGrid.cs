using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Voxelization
{
    /// <summary>
    /// Represents a 3D grid of voxels
    /// Voxel with coordinate (x,y,z) goes from (x,y,z) to (x+1,y+1,z+1)
    /// </summary>
    public class VoxelGrid
    {
        public Point3 Min { get; private set; }
        public Point3 Max { get; private set; }


        private bool[, ,] voxels;

        public VoxelGrid(Point3 min, Point3 max)
        {
            this.Min = min;
            this.Max = max;

            var diff = max - min;

            voxels = new bool[diff.X + 1, diff.Y + 1, diff.Z + 1];
        }

        public bool this[int x, int y, int z]
        {
            get { return voxels[x - Min.X, y - Min.Y, z - Min.Z]; }
            set
            {
                voxels[x - Min.X, y - Min.Y, z - Min.Z] = value;
            }
        }


    }
}
