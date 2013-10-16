using System;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.SkyMerchant.Gameplay;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using MHGameWork.TheWizards.SkyMerchant._Engine.Voxels;
using NUnit.Framework;
using SlimDX;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Stable
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

       /// <summary>
        /// Tests rendering an island as an entity
        /// </summary>
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

        /// <summary>
        /// Generates the heightmap for the bottom of an island
        /// </summary>
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



        /// <summary>
        /// Tests island generation
        /// </summary>
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


        /// <summary>
        /// Renders 100 islands!
        /// </summary>
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

    }
}
