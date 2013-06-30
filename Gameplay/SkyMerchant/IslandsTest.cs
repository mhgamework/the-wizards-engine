using System;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using NUnit.Framework;

namespace SkyMerchantTests
{
    [TestFixture]
    [EngineTest]
    public class IslandsTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

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

            engine.AddSimulator(new WorldRenderingSimulator());
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

            engine.AddSimulator(new WorldRenderingSimulator());

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
                        island.SetVoxel(new Point3(j, -i+size-1, k), dirtMaterial);
                    }
                }
            }

            var c = new VoxelMeshBuilder();
            var mesh = c.BuildMesh(island);


            var e = new Entity();
            e.Mesh = mesh;

            engine.AddSimulator(new WorldRenderingSimulator());


        }

        [Test]
        public void TestGenerateIsland()
        {
            var gen = new IslandGenerater();
            var islandVoxels = gen.GenerateIsland(10);

        }



        [Test]
        public void TestRender100Islands()
        {
            var gen = new IslandGenerater();
            var island = gen.GenerateIsland(10);
        }

        [Test]
        public void TestMoveIsland()
        {

        }
    }

    public class IslandGenerater
    {
        public IFiniteVoxels GenerateIsland(int i)
        {
            throw new NotImplementedException();
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
