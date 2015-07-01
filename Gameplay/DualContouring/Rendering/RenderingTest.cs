using DirectX11;
using MHGameWork.TheWizards.DualContouring.Terrain;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Rendering.Deferred;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring.Rendering
{
    [EngineTest]
    [TestFixture]
    public class RenderingTest
    {
        [Test]
        public void TestSphere()
        {
            var grid = createSphereGrid();

            var dRenderer = TW.Graphics.AcquireRenderer();
            var surfaceRenderer = new VoxelCustomRenderer(TW.Graphics,
                dRenderer,
                new DualContouringMeshBuilder(),
                new DualContouringAlgorithm(), 
                new MeshRenderDataFactory(TW.Graphics,null,dRenderer.TexturePool));
            dRenderer.AddCustomGBufferRenderer(surfaceRenderer);

            surfaceRenderer.CreateSurface( grid, Matrix.Identity );

            EngineFactory.CreateEngine().AddSimulator( new WorldRenderingSimulator() );
        }

        private AbstractHermiteGrid createSphereGrid()
        {
            return HermiteDataGrid.CopyGrid(new DensityFunctionHermiteGrid(
                v => DensityHermiteGridTest.SphereDensityFunction(v, 7, new Vector3(10, 10, 10)), new Point3(20, 20, 20)));
        }
    }
}