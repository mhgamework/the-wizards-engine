using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Rendering
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
