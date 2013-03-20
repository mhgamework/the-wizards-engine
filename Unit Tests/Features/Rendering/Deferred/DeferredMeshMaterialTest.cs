using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.Deferred
{
    /// <summary>
    /// Tests the DeferredMesh.fx shader.
    /// </summary>
    [TestFixture]
    public class DeferredMeshMaterialTest
    {
        [Test]
        public void TestNoMaps() { testGBufferSphere(null, null, null); }
        [Test]
        public void TestDiffuseMap() { testGBufferSphere(RenderingTestsHelper.GetDiffuseMap(), null, null); }
        [Test]
        public void TestNormalMap() { testGBufferSphere(null, RenderingTestsHelper.GetNormalMap(), null); }
        [Test]
        public void TestSpecularMap() { testGBufferSphere(null, null, RenderingTestsHelper.GetSpecularMap()); }

    

        /// <summary>
        /// Renders a test where the GBuffer is displayed containing a sphere with provided maps.
        /// </summary>
        private void testGBufferSphere(ITexture diffuse, ITexture normal, ITexture specular)
        {

        }

        private DX11Game createGame()
        {
            return new DX11Game();
        }

    }
}