using MHGameWork.TheWizards.Rendering;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Rendering
{
    [TestFixture]
    public class MeshLoaderTest
    {
        [Test]
        public void TestLoadMesh()
        {
            var mesh = MeshLoader.LoadMeshFromObj(new System.IO.FileInfo(TestFiles.BarrelObj));



        }
    }
}
