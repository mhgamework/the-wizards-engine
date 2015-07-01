using DirectX11;
using MHGameWork.TheWizards.DualContouring.Terrain;
using MHGameWork.TheWizards.Engine.Features.Testing;
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
            createSphereGrid();
        }

        private AbstractHermiteGrid createSphereGrid()
        {
            return HermiteDataGrid.CopyGrid(new DensityFunctionHermiteGrid(
                v => DensityHermiteGridTest.SphereDensityFunction(v, 7, new Vector3(10, 10, 10)), new Point3(20, 20, 20)));
        }
    }
}