using System;
using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.SkyMerchant.DataStructures;
using MHGameWork.TheWizards.SkyMerchant.Voxels;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;
using SlimDX;
using TreeGenerator.NoiseGenerater;
using TreeGenerator.TerrrainGeneration;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace MHGameWork.TheWizards.SkyMerchant
{
    [TestFixture]
    [EngineTest]
    public class IslandsTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [SetUp]
        public void Setup()
        {
            engine.AddSimulator(new WorldRenderingSimulator());

            var l = TW.Graphics.AcquireRenderer().CreateDirectionalLight();
            l.LightDirection = SlimDX.Vector3.Normalize(new SlimDX.Vector3(-1, -5, -1));
        }

        [Test]
        public void TestRenderEmptyVoxel()
        {
            var dirtMaterial = new VoxelMaterial();
            dirtMaterial.Texture = TW.Assets.LoadTexture("GrassGreenTexture0006.jpg");
            var island = new ArrayFiniteVoxels(new Point3(1, 1, 1));

            var c = new VoxelMeshBuilder();
            var mesh = c.BuildMesh(island);


            var e = new Entity();
            e.Mesh = mesh;
        }

        [Test]
        public void TestRenderSingleVoxel()
        {
            var dirtMaterial = new VoxelMaterial();
            dirtMaterial.Texture = TW.Assets.LoadTexture("GrassGreenTexture0006.jpg");
            var island = new ArrayFiniteVoxels(new Point3(1, 1, 1));
            island.SetVoxel(new Point3(0, 0, 0), dirtMaterial);

            var c = new VoxelMeshBuilder();
            var mesh = c.BuildMesh(island);


            var e = new Entity();
            e.Mesh = mesh;
        }

        [Test]
        public void TestRenderIsland()
        {
            var dirtMaterial = new VoxelMaterial();
            dirtMaterial.Texture = TW.Assets.LoadTexture("GrassGreenTexture0006.jpg");
            var size = 20;
            var island = new ArrayFiniteVoxels(new Point3(size, size, size));
            island.NodeSize = 0.25f;
            for (int i = 0; i < size; i++)
            {
                for (int j = i; j < size - i; j++)
                {
                    for (int k = i; k < size - i; k++)
                    {
                        island.SetVoxel(new Point3(j, -i + size - 1, k), dirtMaterial);
                    }
                }
            }

            var c = new VoxelMeshBuilder();
            var mesh = c.BuildMesh(island);


            var e = new Entity();
            e.Mesh = mesh;

        }

        [Test]
        public void TestGenerateBottomIslandHeightmap()
        {
            var gen = new IslandGenerater(new Random(123));
            var map2 = gen.GenerateBottomIslandHeightmap(32);
            var island = map2.Copy();
            gen.MakeIslandShape(island);


            Action<Array2D<float>> renderLines = delegate(Array2D<float> map)
                {
                    Func<int, int, SlimDX.Vector3> getPoint = (x, y) => new SlimDX.Vector3(x, map[new Point2(x, y)], y);

                    for (int x = 0; x < map.Size.X; x++)
                    {
                        for (int y = 0; y < map.Size.Y; y++)
                        {
                            TW.Graphics.LineManager3D.AddLine(getPoint(x, y), getPoint(x + 1, y), new Color4(0, 0, 0));
                            TW.Graphics.LineManager3D.AddLine(getPoint(x, y), getPoint(x + 1, y + 1),
                                                              new Color4(0, 0, 0));
                            TW.Graphics.LineManager3D.AddLine(getPoint(x, y), getPoint(x, y + 1), new Color4(0, 0, 0));
                        }
                    }
                };

            engine.AddSimulator(new BasicSimulator(delegate
                {
                    renderLines(map2);
                    TW.Graphics.LineManager3D.WorldMatrix = Matrix.Translation(40, 0, 0);

                    renderLines(island);
                }));



        }



        [Test]
        public void TestGenerateIsland()
        {
            var gen = new IslandGenerater(new Random(123));
            var island = gen.GenerateIsland(20);

            var c = new VoxelMeshBuilder();
            var mesh = c.BuildMesh(island);


            var e = new Entity();
            e.Mesh = mesh;

        }



        [Test]
        public void TestRender100Islands()
        {
            var s = new Seeder(123);
            var gen = new IslandGenerater(new Random(123));

            var c = new VoxelMeshBuilder();

            var scale = 25;

            var range = new Vector3(100, 10, 100) * (float)Math.Sqrt(scale);

            for (int i = 0; i < 20 * scale; i++)
            {
                var island = gen.GenerateIsland(s.NextInt(7, 20));
                var mesh = c.BuildMesh(island);
                var e = new Entity();
                e.Mesh = mesh;
                e.WorldMatrix = Matrix.Translation(s.NextVector3(-range, range).dx());
            }


        }

        [Test]
        public void TestMoveIsland()
        {

        }
    }

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
    public class ArrayFiniteVoxels : IFiniteVoxels
    {
        private Array3D<VoxelMaterial> arr;

        public ArrayFiniteVoxels(Point3 size)
        {
            arr = new Array3D<VoxelMaterial>(size);
            NodeSize = 1;
        }
        public void SetVoxel(Point3 pos, VoxelMaterial dirtMaterial)
        {
            arr[pos] = dirtMaterial;
        }

        public Point3 Size { get { return arr.Size; } }
        public float NodeSize { get; set; }
        public VoxelMaterial GetVoxel(Point3 pos)
        {
            return arr[pos];
        }

        public bool IsOutside(Point3 copy)
        {
            return !arr.InArray(copy);
        }
    }
}
