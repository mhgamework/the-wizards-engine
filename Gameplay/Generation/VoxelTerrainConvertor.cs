using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using SlimDX;

namespace MHGameWork.TheWizards.Generation
{
    /// <summary>
    /// Reponsible for converting various formats of data to the TW voxel structure
    /// </summary>
    public class VoxelTerrainConvertor
    {
        public void SetTerrain(bool[, ,] filled)
        {
            int width = (filled.GetUpperBound(0) + 1);
            int height = (filled.GetUpperBound(2) + 1);
            int maxY = filled.GetUpperBound(1) + 1;

            int chunksX = (width / 16) + 1;
            int chunksY = (height / 16) + 1;

            for (int x = 0; x < chunksX; x++)
            {
                for (int y = 0; y < chunksY; y++)
                {
                    var terr = new VoxelTerrainChunk();
                    terr.Size = new Point3(16, 64, 16);
                    //terr.Size = new Point3(5, 5, 5);
                    terr.WorldPosition = Vector3.Modulate(terr.Size.ToVector3() * terr.NodeSize, new Vector3(x, 0, y));
                    terr.Create();

                    for (int tx = 0; tx < terr.Size.X; tx++)
                    {
                        for (int ty = 0; ty < terr.Size.Y; ty++)
                        {
                            for (int tz = 0; tz < terr.Size.Z; tz++)
                            {
                                var heightMapX = tx + (int)terr.WorldPosition.X;
                                var heightMapZ = tz + (int)terr.WorldPosition.Z;
                                if (heightMapX >= width || heightMapZ >= height) continue;
                                if (ty >= maxY) continue;

                                terr.GetVoxelInternal(new Point3(tx, ty, tz)).Filled = filled[heightMapX, ty, heightMapZ];
                            }
                        }
                    }
                }
            }
        }
    }
}
