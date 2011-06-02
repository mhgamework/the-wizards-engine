using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.ServerClient.Terrain;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Terrain.Geomipmap
{
    public static class HeightmapNormalCalculator
    {

        public static Vector3 CalculateAveragedNormal(HeightMap map, int x, int z)
        {
            Vector3 normal = new Vector3();

            // top left
            if (x > 0 && z > 0)
                normal += CalculateNormal(map, x - 1, z - 1);

            // top center
            if (z > 0)
                normal += CalculateNormal(map, x, z - 1);

            // top right
            if (x < map.Width && z > 0)
                normal += CalculateNormal(map, x + 1, z - 1);

            // middle left
            if (x > 0)
                normal += CalculateNormal(map, x - 1, z);

            // middle center
            normal += CalculateNormal(map, x, z);

            // middle right
            if (x < map.Width)
                normal += CalculateNormal(map, x + 1, z);

            // lower left
            if (x > 0 && z < map.Length)
                normal += CalculateNormal(map, x - 1, z + 1);

            // lower center
            if (z < map.Length)
                normal += CalculateNormal(map, x, z + 1);

            // lower right
            if (x < map.Width && z < map.Length)
                normal += CalculateNormal(map, x + 1, z + 1);

            return Vector3.Normalize(normal);
        }

        public static Vector3 CalculateNormal(HeightMap map, int x, int z)
        {
            float scale = 1;
            float heightScale = 1;
            Vector3 v1 = new Vector3(x * scale, map[x, z + 1] * heightScale, (z + 1) * scale);
            Vector3 v2 = new Vector3(x * scale, map[x, z - 1] * heightScale, (z - 1) * scale);
            Vector3 v3 = new Vector3((x + 1) * scale, map[x + 1, z] * heightScale, z * scale);
            Vector3 v4 = new Vector3((x - 1) * scale, map[x - 1, z] * heightScale, z * scale);

            return Vector3.Normalize(Vector3.Cross(v1 - v2, v3 - v4));
        }
    }
}
