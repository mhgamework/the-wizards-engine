using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient.Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.ServerClient.Terrain;

namespace MHGameWork.TheWizards.Terrain.Editor
{
    /// <summary>
    /// Parts where removed here, this is not to be used ,go look in the project's history
    /// </summary>
    public class TerrainRaiseTool
    {


        public static void RaiseTerrain(TerrainFullData terrainData, float x, float z, float range, float height)
        {

            //Vector3 transformed = Vector3.Transform( new Vector3( position.X, 0, position.Y ), Matrix.Invert( WorldMatrix ) );
            //Vector2 localPosition = new Vector2( transformed.X, transformed.Z );

            float SizeX = terrainData.SizeX + 1;
            float SizeY = terrainData.SizeZ + 1;

            x -= terrainData.Position.X;
            z -= terrainData.Position.Z;


            int minX = (int)Math.Floor(x - range);
            int maxX = (int)Math.Floor(x + range) + 1;
            int minZ = (int)Math.Floor(z - range);
            int maxZ = (int)Math.Floor(z + range) + 1;



            if (minX > SizeX - 1) return;
            if (minZ > SizeY - 1) return;
            if (maxX < 0) return;
            if (maxZ < 0) return;

            minX = (int)MathHelper.Clamp(minX, 0, SizeX - 1);
            maxX = (int)MathHelper.Clamp(maxX, 0, SizeX - 1);
            minZ = (int)MathHelper.Clamp(minZ, 0, SizeY - 1);
            maxZ = (int)MathHelper.Clamp(maxZ, 0, SizeY - 1);


            int areaSizeX = maxX - minX + 1;
            int areaSizeZ = maxZ - minZ + 1;

            Rectangle rect = new Rectangle(minX, minZ, areaSizeX, areaSizeZ);


            float rangeSq = range * range;

            for (int ix = minX; ix <= maxX; ix++)
            {
                for (int iz = minZ; iz <= maxZ; iz++)
                {
                    float distSq = Vector2.DistanceSquared(new Vector2(x, z), new Vector2(ix, iz));
                    float iHeight;
                    if (distSq <= rangeSq)
                    {
                        float dist = (float)Math.Sqrt(distSq);
                        float factor = 1 - dist / range;

                        iHeight = terrainData.HeightMap.AddHeight(ix, iz, height * factor);


                    }
                    else
                    {
                        iHeight = terrainData.HeightMap.GetHeight(ix, iz);
                    }
                }

            }


        }

    }
}