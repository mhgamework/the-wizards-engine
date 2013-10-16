using System;
using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.SkyMerchant.DataStructures;
using MHGameWork.TheWizards.SkyMerchant.Generation;
using MHGameWork.TheWizards.SkyMerchant.Voxels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TreeGenerator.NoiseGenerater;
using TreeGenerator.TerrrainGeneration;
using Vector4 = SlimDX.Vector4;

namespace MHGameWork.TheWizards.SkyMerchant.Gameplay
{
    /// <summary>
    /// Generates an island by creating inverting a heightmap and some other manipulations
    /// </summary>
    public class IslandGenerater
    {
        private readonly Random random;

        public IslandGenerater(Random random)
        {
            this.random = random;
        }

        public IFiniteVoxels GenerateIsland(int size)
        {
            var mat = new VoxelMaterial();
            var map = GenerateBottomIslandHeightmap(size);
            MakeIslandShape(map);

            var max = 0f;
            for (int x = 0; x < map.Size.X; x++)
                for (int y = 0; y < map.Size.Y; y++)
                {
                    var val = map[new Point2(x, y)];
                    max = val > max ? val : max;
                }





            var ret = new ArrayFiniteVoxels(new Point3(map.Size.X, (int)Math.Floor(max) + 1, map.Size.Y));

            for (int x = 0; x < map.Size.X; x++)
                for (int y = 0; y < map.Size.Y; y++)
                {
                    var val = map[new Point2(x, y)];
                    for (int i = -(int)val; i < -2; i++)
                    {
                        ret.SetVoxel(new Point3(x, i + ret.Size.Y - 1, y), mat);
                    }
                }

            return ret;

        }

        public void MakeIslandShape(Array2D<float> arr)
        {
            var mid = arr.Size.ToVector2() * 0.5f;
            var deviation = mid.X * 2;
            Func<float, float> gauss = p => (float)Math.Exp(-(p * p) / (2 * deviation));

            var m = arr.Size.X;
            var maxHeight = 1;

            var a = maxHeight / (m * m / 4f);


            Func<float, float> func = p => -p * (p - arr.Size.X) * a;



            for (int x = 0; x < arr.Size.X; x++)
                for (int y = 0; y < arr.Size.Y; y++)
                {
                    var centralized = new SlimDX.Vector2(x, y) - mid;
                    var factor = func(x) * func(y);
                    var value = arr[new Point2(x, y)];
                    var amount = 0f;
                    arr[new Point2(x, y)] = (value * factor) * (1 - amount) + amount * value;
                    //arr[new Point2(x, y)] = func(x) * func(y);

                }
        }

        public Array2D<float> GenerateBottomIslandHeightmap(int size)
        {
            PerlinNoiseGenerater noise;
            noise = new PerlinNoiseGenerater();
            float factor = 0.1f;
            float scale = 10f;
            List<Vector3> positions = new List<Vector3>();
            List<Color> colors = new List<Color>();
            List<Vector3> positionsIsland = new List<Vector3>();
            List<Color> colorsIsland = new List<Color>();
            List<Vector3> positionsFinal = new List<Vector3>();
            List<Color> colorsFinal = new List<Color>();
            int width = size;
            int height = size;
            float lengthFactor = 0.2f;
            SimpleTerrain terrain;
            SimpleTerrain terrainIsland;
            SimpleTerrain terrainFinal;

            ProceduralHeigthGenerater gen = new ProceduralHeigthGenerater(8, 0.7f);
            float heigestFBM = 0;
            float heigestRidge = 0;

            float lowestIsland = 0;
            var heights = new Array2D<float>(new Point2(width, height));
            float[,] heightDataRidge = new float[width, height];
            float[,] heightDataFBM = new float[width, height];
            float[,] heightDataIsland = new float[width, height];
            float[,] heightDataFinal = new float[width, height];


            var r = random;

            var octaves = new[]
                {
                    //new Vector4(1/2f, 0.5f, (float)r.NextDouble(), (float)r.NextDouble()),
                    //new Vector4(1/5f, 2, (float)r.NextDouble(), (float)r.NextDouble()),
                    //new Vector4(1/10f, 5f, (float)r.NextDouble(), (float)r.NextDouble()),
                    //new Vector4(1f, 1f, (float)r.NextDouble(), (float)r.NextDouble()),
                    new Vector4(1/2f, 1f, (float)r.NextDouble(), (float)r.NextDouble()),
                    new Vector4(1/5f, 4, (float)r.NextDouble(), (float)r.NextDouble()),
                    new Vector4(1/10f, 10f, (float)r.NextDouble(), (float)r.NextDouble()),
                };


            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    noise.NumberOfOctaves = 1;

                    //heights[new Point2(i, j)] += noise.RidgedMF(i * lengthFactor, j * lengthFactor, 0.15f, 8, 2f, 0.7f, 0.8f);//noise.CombinedFractalBrowningAndRidgedMF(i * lengthFactor, j * lengthFactor, 8, 4,5f, 0.5f, 2f, 0.8f, 1f);


                    foreach (var v in octaves)
                    {
                        float frequency = v.X;
                        float oScale = v.Y;
                        Vector2 offset = new Vector2(v.Z, v.W) * 1000;

                        heights[new Point2(i, j)] += noise.GetPerlineNoise((i + offset.X) * frequency, (j + offset.Y) * frequency) * oScale;


                    }

                    heightDataFBM[i, j] = noise.GetFractalBrowningNoise(i * lengthFactor, j * lengthFactor, 8, 1.2f, 1.9f, 1.2f);
                    heightDataIsland[i, j] = gen.IslandFactor(i * lengthFactor, j * lengthFactor, new Vector2(width * lengthFactor * 0.5f, height * lengthFactor * 0.5f), width * lengthFactor * 0.42f, width * lengthFactor * 0.4f);//noise.CombinedFractalBrowningAndRidgedMF(i, j, 8, 4, 4, 0.9f, 0.5f, 1.2f, 0.8f)*0.1f+gen.IslandFactor(i, j, new Vector2(width * 0.45f, height * 0.5f),0,width*0.22f)*0.9f  ;

                    if (heigestFBM < heightDataFBM[i, j])
                    {
                        heigestFBM = heightDataFBM[i, j];
                    }
                    if (heigestRidge < heightDataRidge[i, j])
                    {
                        heigestRidge = heightDataRidge[i, j];
                    }

                    if (lowestIsland > heightDataIsland[i, j])
                    {
                        lowestIsland = heightDataIsland[i, j];
                    }
                }
            }


            return heights;
        }
    }
}