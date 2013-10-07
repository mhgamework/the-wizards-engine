using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.SkyMerchant.Generation;
using MHGameWork.TheWizards.SkyMerchant.Voxels;
using NUnit.Framework;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Stable
{
    [TestFixture]
    [EngineTest]
    public class VoxelsTest
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
        /// Test converting an ArrayFiniteVoxels with no voxels to a mesh
        /// </summary>
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

        /// <summary>
        /// Test converting an ArrayFiniteVoxels with one voxel to a mesh
        /// </summary>
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

    }

}
