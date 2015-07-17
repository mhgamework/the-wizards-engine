using DirectX11;
using MHGameWork.TheWizards.DualContouring.Terrain;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred.Meshes;
using MHGameWork.TheWizards.Rendering.Deferred;
using NUnit.Framework;
using SlimDX;
using VoxelEngine.Tests;

namespace MHGameWork.TheWizards.DualContouring.Rendering
{
    public class RenderingTest:EngineTestFixture
    {
        private VoxelCustomRenderer surfaceRenderer;

        [SetUp]
        public void Setup()
        {
            var dRenderer = TW.Graphics.AcquireRenderer();
            surfaceRenderer = new VoxelCustomRenderer(TW.Graphics,
                                                      dRenderer,
                                                      new DualContouringMeshBuilder(),
                                                      new DualContouringAlgorithm(),
                                                      new MeshRenderDataFactory(TW.Graphics, null, dRenderer.TexturePool));
            dRenderer.AddCustomGBufferRenderer(surfaceRenderer);
            EngineFactory.CreateEngine().AddSimulator(new WorldRenderingSimulator());

        }

        [Test]
        public void Test2Materials()
        {
            var mat1 = new DCVoxelMaterial() { Texture = DCFiles.UVCheckerMap10_512 };
            var mat2 = new DCVoxelMaterial() { Texture = DCFiles.UVCheckerMap11_512 };

            var mesh1 =
                new DensityFunctionHermiteGrid(
                    v => DensityHermiteGridTest.SphereDensityFunction(v, 2, new Vector3(2)), new Point3(10, 10, 10),
                    v => mat1);

            var mesh2 =
                new DensityFunctionHermiteGrid(
                    v => DensityHermiteGridTest.SphereDensityFunction(v, 2, new Vector3(2)), new Point3(10, 10, 10),
                    v => mat2);

            surfaceRenderer.CreateSurface(mesh1, Matrix.Translation(0, 0, 0));
            surfaceRenderer.CreateSurface(mesh2, Matrix.Translation(15, 0, 0));

        }

        [Test]
        public void Test2MaterialsUnion()
        {
            var mat1 = new DCVoxelMaterial() { Texture = DCFiles.UVCheckerMap10_512 };
            var mat2 = new DCVoxelMaterial() { Texture = DCFiles.UVCheckerMap11_512 };

            var mesh1 =
                new DensityFunctionHermiteGrid(
                    v => DensityHermiteGridTest.SphereDensityFunction(v, 2, new Vector3(2)), new Point3(20, 20, 20),
                    v => mat1);

            var mesh2 =
                new DensityFunctionHermiteGrid(
                    v => DensityHermiteGridTest.SphereDensityFunction(v, 2, new Vector3(2)), new Point3(10, 10, 10),
                    v => mat2);

            var grid = new UnionGrid( mesh1, mesh2, new Point3( 1, 1, 1 ) );



            surfaceRenderer.CreateSurface(grid, Matrix.Translation(0, 0, 0));

        }

        [Test]
        public void TestSphere()
        {
            var grid = createSphereGrid();



            surfaceRenderer.CreateSurface(grid, Matrix.Identity);

        }

        private AbstractHermiteGrid createSphereGrid()
        {
            return HermiteDataGrid.CopyGrid(new DensityFunctionHermiteGrid(
                v => DensityHermiteGridTest.SphereDensityFunction(v, 7, new Vector3(10, 10, 10)), new Point3(20, 20, 20)));
        }
    }
}